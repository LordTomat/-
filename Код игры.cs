Это код игры "Традиции нашего племени" (представлены одним exe-файлом и кучей текстовых и графических вне его)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ТрадицииНашегоПлемени
{
    public partial class Form1 : Form
    {
        WMPLib.WindowsMediaPlayer Player;
        int rFood; //Еда
        int rPeople; //Кол. людей в поселении
        int rHeal; //Лекарства в поселении
        int rWeapon; //Кол. оружия в поселении
        int rWood; //Кол. примитивных ресурсов 
        int rMetal; //Кол металлического хлама
        int rGold; //Кол. драгоценностей
        int rMorale; //Психологическое состояние поселения
        int Day = -1; //Сколько дней прошло в игре
        int RepTribe; //Репутация с ближайшим племенем
        int RepCity; //Репутация с ближайшим индустриальным городом
        int RepCastle; //Репутация с ближайшим замком
        bool Show = true; //Тестовая переключался для показания/убирание кнопок и ивентовых штук
        string TextSave; //Используется для сохранения
        int EventN = 0; //Айдишник ивента. Нулевой - старт
        Random rnd = new Random();
        byte[] EventCheck = new byte[200]; //Массив - айди уникального ивента, проверяет, активирова ли он...
        bool EventU = false; //Если true - то сейчас идёт уникальный ивент, не отнимает дни, не отнимает еду...
        int N; int U; int Y; int T; int B; int R; int P; //Типо локальные переменные, но во время реализации case оно не хочет их "делить" с другими, поэтому делаю их в таком виде
        //Чтоб не забыть. rnd.Next(1, 4) = случайное от 1 до 3, т.е. последнее -1 по какой то причине. (потому что счёт начинается с 0, балда!)
        bool[] HaveAch = new bool[20]; //Запоминает, получил ли игрок то или иное достижение
        bool[] HaveStat = new bool[20]; //Запоминает, есть ли у игрока тот или иной статусный эффект
        int[] RemembaStat = new int[20]; //Для запомнинаия особых значнеий, чтоб потом юзать на ачивки (например считать кол. боёв
        string[] StatText = { "Запасы еды", "Хорошая подготовка", "Неизвестная болезнь", "Домашний скот", "Замороженный конфликт", "Каменные стены", "Война на границах", "Доступ к королевским лесам" };
        string[] AchText = { "1 год", "Манса Муса", "Первобытный коммунизм", "Горшочек, не вари!", "Последователи Штирнера", "Цельнометаллическая заточка" };
        string[] AchDesc = { "Ваше племя смогло выжить в течении года, поразительная живучесть, учитывая что происходит каждый день.", "Вы добыли безумно больше количество драгоценностей для этих мест, можете не экономить до конца жизни!", "Кажется, у вас получилось создать ту самую идею сплочённости, однако, у вас совсем туго с едой... Издержки режима", "У вас скопилось так много еды, что в племени тема фатшейминга поднимается чаще всего остального.", "Вы никому ничего не обязаны и делайте то, что считайте нужным и по силам. Отношения с другими фракциями - дело вторичное.", "Вы, при не очень равных технологических условиях, пережили аж 5 атак и защит. Похвально." };
        List<Control> list_labels = new List<Control>();
        List<Control> list_labels2 = new List<Control>();
        byte DevLVL = 0; //Уровень развития поселения. 0 - племя, 1 - замок, 2 - город, 3 -космос. Последние 2 вряд ли будут доступны игроку, но на всякий...
        byte EnemyDevLVL = 0; //Используется для битв
        int EnemyNum; //Используется для битв как подсчёт кол. врагов
        int PlaceBattle; //Айди места битвы
        byte TimeBattle; //Айди времени битвы
        bool BattleGo = false; //Проверяет, идёт ли битва. Сделал, дабы ненароком нельзя было тыкать на кнопки за полем битвы
        int IDT; //Айдишник атаки. Запоминает, какую тактику прожал игрок и реагирует на неё
        int IDD; //Айдишник события. Запоминает, что происходит вообще на поле боя
        string[] BattleLogs = { "Вражеские войска идут в лобовую атаку на открытом пространстве...", }; //Текст для события                     я не помню где это используется, лол
        int r1; int r2; int r3; int r4; int r5; int r6; int r7; int r8; //Используются для запоминания бонусов от нажатия выбора
        bool BattleType = false; //Проверяет, защитник ли игрок в этой битве. Если true - дать положение защитника, иначе flase - он атакующий
        int BattleNumEnemy; //Сколько врагов атакуют в данный момент (реворк боевой системы)
        int BattleNumOur; //Тоже самое, но подсчёт наших
        byte Lang = 0; //Айдишник языка, используется для загрузки текста
        String[] EvnDesc = new string[1200]; //Используется для запоминания описания. Номер массива = номер ивента.
        String[] EvnButt1 = new string[1200]; //Используется для запоминания описания кнопки. 
        String[] EvnButt2 = new string[1200]; String[] EvnButt3 = new string[1200]; String[] EvnButt4 = new string[1200]; String[] EvnButt5 = new string[1200]; String[] EvnButt6 = new string[1200];
        String[] Word = new string[1200]; //Запоминает все слова для перевода, вроде "Победа" или " продать алмазы за ", "описания атаки" и прочие тексты
        int lenght = 120; //Задаёт длину переноса текста в ивентах
        string SS = ""; //Строка-исходник. Сюда запоминается главный текст
        string RS = ""; //Строка-выход, в конце её отображаем
        bool MusicON = true; //Вкл/выкл музыка. Скопировал с беллумки но пока не использую
        int[] FoodIvn = { 3, 6, 10, 17, 18, 26, 28 }; //Короче если еды мало - игра "пытается" подкинуть айди ивента из этого массива, т.е. ивента, связанного с едой
        int[] ResIvn = { 2, 14, 20, 22, 25, 26}; //Аналогично высшему, но с ресами
        int[] WeapIvn = { 12, 13, 30, 31 }; //С оружием
        int[] PeopIvn = { 1, 12, 18, 21, 29 };
        int NumOfWord = 128; //Используется для поиска слов по переведу. Следующее число за ним - массив из статус-эффектов и ачивок
        Button[] Butt = new Button[10]; //Массив кнопок, первый раз делал для консоли
        bool ConOpen = false; //Проверяет, открыта ли консоль или нет. Заменить при первой же возможности на что то другое!
        TextBox Consol; //ТекстБокс для консоли (куда вводятся слова)
        PictureBox PanelBack = new PictureBox(); //Тестирую накладывание картинок друг на друга чтоб создать более эффектные приколы
        public Form1()
        {
            InitializeComponent();
            LangTextBox.Text = "rus.txt";
            this.BackgroundImage = Image.FromFile("Image/TribeMain.png");
            for (int i = 0; i < EvnButt1.Length; i++)
            {
                EvnDesc[i] = "";
                EvnButt1[i] = "";
                EvnButt2[i] = "";
                EvnButt3[i] = "";
                EvnButt4[i] = "";
                EvnButt5[i] = "";
                EvnButt6[i] = "";
            }
            Lang = 0;
            LoadText();
            Music.Start();
            NumDay.Visible = false;
            SaveGame.Visible = false;
            StartGame.Location = new Point(650, 25);
            LoadGame.Location = new Point(650, 75);
            NumFood.Visible = false;
            NumGold.Visible = false;
            NumHeal.Visible = false;
            NumMetal.Visible = false;
            NumMorale.Visible = false;
            NumPeople.Visible = false;
            NumWeapon.Visible = false;
            NumWoodStone.Visible = false;
            TradeTrack.Visible = false;
            PanelStatus.Visible = false;
            ButtStat.Visible = false;
            PanelBattle.Visible = false;
            ShutDownString();
            HideGameplay();
            System.Console.WriteLine(Word[0]);
            list_labels.Add(label1); list_labels.Add(label2); list_labels.Add(label3); list_labels.Add(label4); list_labels.Add(label5); list_labels.Add(label6); list_labels.Add(label7); list_labels.Add(label8); list_labels.Add(label9); list_labels.Add(label10); list_labels.Add(label11); list_labels.Add(label12); list_labels.Add(label13); list_labels.Add(label14); list_labels.Add(label15); list_labels.Add(label16); list_labels.Add(label17); list_labels.Add(label18); list_labels.Add(label19); list_labels.Add(label20);
            list_labels2.Add(Status1); list_labels2.Add(Status2); list_labels2.Add(Status3); list_labels2.Add(Status4); list_labels2.Add(Status5); list_labels2.Add(Status6); list_labels2.Add(Status7); list_labels2.Add(Status8); list_labels2.Add(Status9); list_labels2.Add(Status10); list_labels2.Add(Status11); list_labels2.Add(Status12);
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            LoadText();
            Starting();
        }

        private void LoadText()
        {
            string Pyt = LangTextBox.Text; //Путь к файлу
            if (System.IO.File.Exists(Pyt) == false)
            {
                MessageBox.Show("Language file not found. English file will be loaded!");
                Pyt = "eng.txt";
            }
            using (StreamReader file = new StreamReader(Pyt))
            {
                System.Console.WriteLine("Начинаем загружать язык");
                int counter = 0; //Запоминает строку
                int counter2 = 0; //запоминает айдишник ^ при добавлении
                string ln;
                byte Str = 0; //Запоминает строку, от которой надо отталкиваться
                int Id = -1; //Запоминает айдишник ивента в который записать текст
                bool End = false; //Проверяет, кончилась ли строка с ивентами

                while ((ln = file.ReadLine()) != null)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(ln, "="))
                    {
                        System.Console.WriteLine("Я нашёл = на " + counter + " строке");
                        Id = Convert.ToInt32(ln.Substring(1));
                        System.Console.WriteLine("Достали оттуда Id");
                        Str = 0;
                    }
                    else
                    {
                        if (End == false)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(ln, "#"))
                            {
                                End = true;
                                System.Console.WriteLine("Переключили на обычный текст");
                            }
                            else
                            {
                                switch (Convert.ToString(Str))
                                {
                                    case "0":
                                        EvnDesc[Id] = Convert.ToString(ln);
                                        System.Console.WriteLine("Текст описания: " + ln);
                                        break;
                                    case "1":
                                        EvnButt1[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                    case "2":
                                        EvnButt2[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                    case "3":
                                        EvnButt3[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                    case "4":
                                        EvnButt4[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                    case "5":
                                        EvnButt5[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                    case "6":
                                        EvnButt6[Id] = ln;
                                        System.Console.WriteLine("Добавил описание кнопки " + Convert.ToString(Str) + " ивента №" + Id + ": " + ln);
                                        break;
                                }
                                Str++;
                            }
                        }
                        else
                        {
                            Word[counter2] = ln;
                            System.Console.WriteLine("Добавил в список под номером " + counter2 + " слова: " + ln);
                            counter2++;
                        }
                    }
                    counter++;
                }
                file.Close();
            }
            System.Console.WriteLine("Текст загрузился");
            System.Console.WriteLine("Описание нулевого " + EvnDesc[0]);
        }

        private void LoadGame_Click(object sender, EventArgs e)
        {
            Starting();
            string Text = File.ReadAllText("Save.txt");
            using (StreamReader file = new StreamReader("Save.txt"))
            {
                int counter = 0;
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    switch (Convert.ToString(counter))
                    {
                        case "0":
                            Day = Int32.Parse(ln) - 1;
                            break;
                        case "1":
                            rPeople = Int32.Parse(ln);
                            break;
                        case "2":
                            rMorale = Int32.Parse(ln);
                            break;
                        case "3":
                            rHeal = Int32.Parse(ln);
                            break;
                        case "4":
                            rFood = Int32.Parse(ln);
                            break;
                        case "5":
                            rGold = Int32.Parse(ln);
                            break;
                        case "6":
                            rWeapon = Int32.Parse(ln);
                            break;
                        case "7":
                            rWood = Int32.Parse(ln);
                            break;
                        case "8":
                            rMetal = Int32.Parse(ln);
                            break;
                        case "9":
                            EventN = Int32.Parse(ln);
                            break;
                        case "10":
                            char[] b = new char[ln.Length];
                            using (StringReader sr = new StringReader(ln))
                            {
                                sr.Read(b, 0, ln.Length);
                                System.Console.WriteLine(b);
                            }
                            for (int i = 0; b.Length > i; i++)
                            {
                                if (b[i] == '1')
                                {
                                    HaveAch[i] = true;
                                }
                                else
                                {
                                    HaveAch[i] = false;
                                }
                            }
                            break;
                        case "11":
                            RepTribe = Int32.Parse(ln);
                            break;
                        case "12":
                            RepCastle = Int32.Parse(ln);
                            break;
                        case "13":
                            RepCity = Int32.Parse(ln);
                            break;
                        case "14":
                            DevLVL = Byte.Parse(ln);
                            break;
                        case "15":
                            char[] g = new char[ln.Length];
                            using (StringReader sr = new StringReader(ln))
                            {
                                sr.Read(g, 0, ln.Length);
                                System.Console.WriteLine(g);
                            }
                            for (int i = 0; g.Length > i; i++)
                            {
                                if (g[i] == '1')
                                {
                                    HaveStat[i] = true;
                                }
                                else
                                {
                                    HaveStat[i] = false;
                                }
                            }
                            break;
                    }
                    counter++;
                }
                file.Close();
            }
            LoadText();
            TextUpdate();
            EventLoad();
        }
        private void SaveGame_Click(object sender, EventArgs e)
        {
            string AchSave = "";
            string StatSave = "";
            for (int i = 0; HaveAch.Length > i; i++)
            {
                if (HaveAch[i] == false)
                {
                    AchSave += "0";
                }
                else
                {
                    AchSave += "1";
                }
            }
            for (int i = 0; HaveStat.Length > i; i++)
            {
                if (HaveStat[i] == false)
                {
                    StatSave += "0";
                }
                else
                {
                    StatSave += "1";
                }
            }
            System.Console.WriteLine("Готово");
            TextSave = Day + "\n" + rPeople + "\n" + rMorale + "\n" + rHeal + "\n" + rFood + "\n" + rGold + "\n" + rWeapon + "\n" + rWood + "\n" + rMetal + "\n" + EventN + "\n" + AchSave + "\n" + RepTribe + "\n" + RepCastle + "\n" + RepCity + "\n" + DevLVL + "\n" + StatSave;
            string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save.txt");
            File.WriteAllText(destPath, TextSave.ToString());
            MessageBox.Show(Word[101]);
        }
        public void Starting()
        {
            LangLeft.Hide();
            LangRight.Hide();
            LangText.Hide();
            LangTextBox.Hide();
            Volume.Visible = false;
            VolumeText.Visible = false;
            this.BackgroundImage = Properties.Resources.Empty;
            for (int i = 0; i < HaveAch.Length; i++)
            {
                HaveAch[i] = false;
            }
            for (int i = 0; i < HaveStat.Length; i++)
            {
                HaveStat[i] = false;
            }
            Random rnd = new Random();
            rPeople = rnd.Next(7, 12);
            rGold = rnd.Next(0, 2);
            rHeal = rnd.Next(4, 11);
            rFood = rnd.Next(12, 20);
            rMetal = rnd.Next(0, 8);
            rWeapon = rnd.Next(3, 8);
            rWood = rnd.Next(7, 12);
            rMorale = rnd.Next(70, 90);
            RepCastle = rnd.Next(200, 501);
            RepTribe = rnd.Next(0, 751);
            RepCity = rnd.Next(300, 351);
            NumFood.Visible = true;
            NumGold.Visible = true;
            NumHeal.Visible = true;
            NumMetal.Visible = true;
            NumMorale.Visible = true;
            NumPeople.Visible = true;
            NumWeapon.Visible = true;
            NumWoodStone.Visible = true;
            StartGame.Visible = false;
            LoadGame.Visible = false;
            SaveGame.Visible = true;
            NumDay.Visible = true;
            ButtStat.Visible = true;
            Version.Visible = false;
            HideGameplay();
            TextUpdate();
            EventLoad();
        }

        public void TextUpdate()
        {
            if (rFood < 0) { rFood = 0; }
            if (rPeople < 0) { rPeople = 0; }
            if (rGold < 0) { rGold = 0; }
            if (rHeal < 0) { rHeal = 0; }
            if (rMetal < 0) { rMetal = 0; }
            if (rMorale < 0) { rMorale = 0; }
            if (rWeapon < 0) { rWeapon = 0; }
            if (rWood < 0) { rWood = 0; }
            NumFood.Text = Word[93] + rFood;
            NumGold.Text = Word[94] + rGold;
            NumHeal.Text = Word[95] + rHeal;
            NumMetal.Text = Word[96] + rMetal;
            NumMorale.Text = Word[97] + rMorale;
            NumPeople.Text = Word[98] + rPeople;
            NumWeapon.Text = Word[99] + rWeapon;
            NumWoodStone.Text = Word[100] + rWood;
            NumDay.Text = Word[92] + Day;
        }

        public void HideGameplay()
        {
            Show = !Show;
            EventButt1.Visible = Show;
            EventButt2.Visible = Show;
            EventButt3.Visible = Show;
            EventButt4.Visible = Show;
            EventButt5.Visible = Show;
            EventButt6.Visible = Show;
            EventPicture.Visible = Show;
            EventText.Visible = Show;
        }

        public void CheckRes()
        {
            if (EventN != 0 && EventU == false)
            {
                if (HaveStat[3] == true) { rFood++; }
                if (HaveStat[4] == true) { rMorale -= 3; ; }
                if (rFood <= 0)
                {
                    if (HaveStat[0] == true)
                    {
                        HaveStat[0] = false;
                        rFood = 10;
                    }
                    else
                    {
                        rPeople--;
                        rMorale -= 5;
                    }
                }
                if (rPeople >= 5)
                {
                    rFood -= (rPeople / 5);
                }
                else
                {
                    rFood--;
                }
                if (rMorale <= 0)
                {
                    rPeople--;
                }
                if (rMorale > 100)
                {
                    rMorale = 100;
                }
                if (RepCastle <= 0)
                {
                    RepCastle = 0;
                }
                if (RepCity <= 0)
                {
                    RepCity = 0;
                }
                if (RepTribe <= 0)
                {
                    RepTribe = 0;
                }
                if (HaveStat[2] == true)
                {
                    if (rHeal >= 1)
                    {
                        rHeal--;
                    }
                    else
                    {
                        rPeople--;
                    }
                    int I = rnd.Next(1, 11);
                    if (I == 10)
                    {
                        HaveStat[2] = false;
                    }
                }
                if (HaveStat[6] == true)
                {
                    int I = rnd.Next(1, 31);
                    if (I == 10)
                    {
                        HaveStat[6] = false;
                    }
                }
                TextUpdate();
                if (rPeople == 0)
                {
                    GG();
                }
                //if (Day == 100) //Временно удалена ввиду появления ачивки за год выживания
                //{
                //    Win();
                //}    
            }
        }
        private void SetRes()
        {
            r1 = rPeople;
            r2 = rFood;
            r3 = rHeal;
            r4 = rMorale;
            r5 = rWeapon;
            r6 = rWood;
            r7 = rMetal;
            r8 = rGold;
        }

        private void BonusSee()
        {
            if ((rPeople - r1) > 0)
            {
                BonPeople.Text = "+" + (rPeople - r1);
            }
            if ((rPeople - r1) < 0)
            {
                BonPeople.Text = "" + (rPeople - r1);
            }
            if ((rFood - r2) > 0)
            {
                BonFood.Text = "+" + (rFood - r2);
            }
            if ((rFood - r2) < 0)
            {
                BonFood.Text = "" + (rFood - r2);
            }
            if ((rHeal - r3) > 0)
            {
                BonHeal.Text = "+" + (rHeal - r3);
            }
            if ((rHeal - r3) < 0)
            {
                BonHeal.Text = "" + (rHeal - r3);
            }
            if ((rMorale - r4) > 0)
            {
                BonMorale.Text = "+" + (rMorale - r4);
            }
            if ((rMorale - r4) < 0)
            {
                BonMorale.Text = "" + (rMorale - r4);
            }
            if ((rWeapon - r5) > 0)
            {
                BonWeapon.Text = "+" + (rWeapon - r5);
            }
            if ((rWeapon - r5) < 0)
            {
                BonWeapon.Text = "" + (rWeapon - r5);
            }
            if ((rWood - r6) > 0)
            {
                BonWood.Text = "+" + (rWood - r6);
            }
            if ((rWood - r6) < 0)
            {
                BonWood.Text = "" + (rWood - r6);
            }
            if ((rMetal - r7) > 0)
            {
                BonMetal.Text = "+" + (rMetal - r7);
            }
            if ((rMetal - r7) < 0)
            {
                BonMetal.Text = "" + (rMetal - r7);
            }
            if ((rGold - r8) > 0)
            {
                BonGold.Text = "+" + (rGold - r8);
            }
            if ((rGold - r8) < 0)
            {
                BonGold.Text = "" + (rGold - r8);
            }
        }

        private void StringPerenos()
        {
            int Test; //Сюда мы запоминаем положение (порядковый номер) последнего пробела
            RS = ""; //чистим результат перед тем, как записывать свежий
            int leng = SS.Length; //Специиальная хуйня, потому что я уже час ебусь с надеждой починить эту простую систему переноса
            System.Console.WriteLine("Начали резать текст: " + SS);
            do
            {
                if (leng <= lenght)
                {
                    RS += "\n" + SS;
                    leng = 0;
                }
                else
                {
                    Test = 0; //Обнуляем координату пробела, типо его нет
                    for (int i = 0; i <= lenght; i++)
                    {
                        if (SS[i] == ' ')
                        {
                            Test = i;
                        }
                    }
                    if (Test == 0)
                    {
                        Test = lenght; //Если мы не нашли пробела - мы тупо режем на лимите
                    }
                    leng -= Test;
                    RS += "\n" + SS.Substring(0, Test);
                    SS = SS.Substring(Test);
                } 
            } while (leng > 1);
        }

        private void EventAlgoritm()
        {
            N = rnd.Next(1, 38); //Установка абсолютно рандомного ивента. Всегда должна быть в самом начале и по факту отсюда ивент и берётся, если другое не рольнет

            if (rWood < 2 && rnd.Next(0, 3) == 1)
            {
                N = ResIvn[rnd.Next(0, ResIvn.Length)];
            } //Если ресов от 1 и меньше, с 33% шансом пытается подкинуть ивент на ресы
            if (rWeapon < 2 && rnd.Next(0, 3) == 1)
            {
                N = WeapIvn[rnd.Next(0, WeapIvn.Length)];
            } //Если оружия от 1 и меньше, с 33% шансом пытается подкинуть ивент на оружие
            if (rFood < 5 && rnd.Next(0,2) == 1)
            {
                N = FoodIvn[rnd.Next(0, FoodIvn.Length)];
            } //Если еды от 4 и меньше, с 50% шансом пытается подкинуть ивент на еду
            if (rPeople < 3 && rnd.Next(0, 2) == 1)
            {
                N = PeopIvn[rnd.Next(0, PeopIvn.Length)];
            } //Если людей от 2 и меньше, с 50% шансом пытается подкинуть ивент на людей

            U = rnd.Next(1, 11); //Рандомизатор, 10% шанс что придёт торгаш
            if (EventU == true) //Если ивент - последовательный, то...
            {
                switch (Convert.ToString(EventN))
                {
                    case "1000": // Последовательность гига-ивентов?
                        EventN = 1001;
                        break;
                }
            }
            if (EventU == false && U == 9 && EventN != 999) 
            {
                EventN = 999;
                T = rnd.Next(1, 8);
                B = rnd.Next(1, 8);
            Check:
                if (B == T)
                {
                    B = rnd.Next(1, 8);
                    goto Check;
                }
                // Ивент с торговцем
                switch (Convert.ToString(T)) //1 - люди, 2 - еда, 3 хилки, 4 - обыч. ресы, 5 - металл, 6 - оружие, 7 - алмазы. T - продаёт, B - требует
                {
                    case "1":
                        R = 1;
                        //1 раб в обмен на
                        break;
                    case "2":
                        R = rnd.Next(2, 5);
                        //2 - 4 пищи в обмен на
                        break;
                    case "3":
                        R = rnd.Next(1, 4);
                        //1 -3 лекарств в обмен на
                        break;
                    case "4":
                        R = rnd.Next(3, 6);
                        //3 - 5 ресурсов в обмен на 
                        break;
                    case "5":
                        R = rnd.Next(2, 5);
                        // 2 - 4 металла в обмен на
                        break;
                    case "6":
                        R = rnd.Next(2, 4);
                        // 2-3 оружия в обмен на
                        break;
                    case "7":
                        R = 1;
                        // 1 алмаз в обмен на
                        break;
                }
                switch (Convert.ToString(B)) //1 - люди, 2 - еда, 3 хилки, 4 - обыч. ресы, 5 - металл, 6 - оружие, 7 - алмазы. T - продаёт, B - требует
                {
                    case "1":
                        P = 1;
                        // раба
                        TradeTrack.Maximum = rPeople - 1;
                        break;
                    case "2":
                        P = rnd.Next(2, 5);
                        // пищу
                        TradeTrack.Maximum = rFood / P;
                        break;
                    case "3":
                        P = rnd.Next(1, 4);
                        // лекарства
                        TradeTrack.Maximum = rHeal / P;
                        break;
                    case "4":
                        P = rnd.Next(3, 6);
                        // ресурсы
                        TradeTrack.Maximum = rWood / P;
                        break;
                    case "5":
                        P = rnd.Next(2, 5);
                        // металл
                        TradeTrack.Maximum = rMetal / P;
                        break;
                    case "6":
                        P = rnd.Next(2, 4);
                        // оружие
                        TradeTrack.Maximum = rWeapon / P;
                        break;
                    case "7":
                        P = 1;
                        // алмаз
                        TradeTrack.Maximum = rGold / P;
                        break;
                }
                TradeTrack.Visible = true;
            } //Торгаш пришёл. Настраиваем его
            else
            {
                if (EventU == false)
                {
                    Y = rnd.Next(1, 101);
                    if (rMorale <= 10 && Y >= 25)
                    {
                        N = rnd.Next(900, 903); //Ивенты на низкую мораль с 75% шансом
                    }
                    if (rFood == 0 && Y >= 25)
                    {
                        N = rnd.Next(800, 801); //Ивенты на голод с 75% шансом
                    }
                    if (RepTribe < 250 && (Y >= 90 || (HaveStat[6] == true && Y >= 80)))
                    {
                        N = 850; //Ивент на войну с 10% шансом при низком отношении с племенем
                    }
                    if (RepCastle < 250 && (Y >= 93 || (HaveStat[6] == true && Y >= 83)))
                    {
                        N = 851; //Ивент на войну с 7% шансом при низком отношении с замком
                    }
                    if (RepCity < 250 && (Y >= 94 || (HaveStat[6] == true && Y >= 84)))
                    {
                        N = 852; //Ивент на войну с 6% шансом при низком отношении с городом ... Дальше идут ивенты на случай, его игрок не имеет какого то показателя при 5-10% шансе 
                    }
                    if (RepCity > 750 && Y >= 91 && HaveStat[1] == false && rGold >= 3)
                    {
                        N = 853; //Город берёт в сферу влияния с 9% шансом
                    }
                    if (RepTribe > 750 && Y >= 91 && HaveStat[3] == false && (rGold >= 4 || rWood >= 8))
                    {
                        N = 854; //Дружеское племя предагает скот с 9% шансом
                    }
                    if (HaveStat[2] == false && Y >= 98 && rPeople >= 3) { N = 855; } //Заразная болезнь с 3% шансом
                    if (HaveStat[5] == false && Y >= 96 && rWood >= 12) { N = 856; } //Построить стену с 5% шансом
                    if (HaveStat[6] == false && Y >= 99 && rPeople >= 1) { N = 857; } //Война с 2% шансом
                    if (RepCastle > 750 && Y >= 92 && HaveStat[7] == false && (rPeople >= 4))
                    {
                        N = 858; //Замок предлагает сделку с 8% шансом 
                    }
                    //if (Y >= 0 && EventCheck[0] == 0)
                    //{
                    //    N = 1000;
                    //    EventU = true;                     //тут типо уникальная цепочка ивентов, но я понял, что от неё говной воняет!
                    //    EventCheck[0] = 1;
                    //}
                    EventN = N;
                }
            } //Торгаш не пришёл? Отменяем? Тут немного стрёмная херня, потому что оно ВРОДЕ бы юзает ту же систему, что и гига ивенты. Обрати внимание, когда начнёт их делать      
        }
        private void EventLoad()
        {
            if (EventU == false)
            { Day++;
                if (HaveStat[7] == true && RemembaStat[1] == 1)
                {
                    rWood++;
                    RemembaStat[1] = 0;
                }
                else
                {
                    RemembaStat[1] = 1;
                }
                if (HaveAch[0] == false && Day >= 365) 
                { 
                    HaveAch[0] = true; 
                } 
                if (HaveAch[1] == false && rGold >= 100)
                {
                    HaveAch[1] = true;
                }
                if (HaveAch[2] == false && rFood <= 0 && rMorale >= 100)
                {
                    HaveAch[2] = true;
                }
                if (HaveAch[3] == false && rFood >= rPeople * 12 && rPeople >= 10)
                {
                    HaveAch[3] = true;
                }
                if (HaveAch[4] == false && RepCity <= 0 && RepTribe <= 0 && RepCastle <= 0)
                {
                    HaveAch[4] = true;
                }
            }  
            NumDay.Text = Word[92] + Day;
            EventButt1.Visible = false;
            EventButt2.Visible = false;
            EventButt3.Visible = false;
            EventButt4.Visible = false;
            EventButt5.Visible = false;
            EventButt6.Visible = false;
            lenght = 125;
            if (EventN == 999)
            {
                System.Console.WriteLine("Текст торгаша таков: " + EvnDesc[EventN]);
            }
            SS = EvnDesc[EventN];
            if (SS.Length > lenght) //Если текст слишком длинный - надо его порезать, иначе...
            {
                StringPerenos();
            }
            else
            {
                RS = SS; //Иначе ничего не режем, лол. А я тут в час ночи сижу и думаю, в чём баг заключается. Без этой херни короткий текст удалялся
            }
            System.Console.WriteLine("Номер ивента: " + EventN);
            EventText.Text = RS;
            EventButt1.Text = EvnButt1[EventN];
            EventButt2.Text = EvnButt2[EventN];
            EventButt3.Text = EvnButt3[EventN];
            EventButt4.Text = EvnButt4[EventN];
            EventButt5.Text = EvnButt5[EventN];
            EventButt6.Text = EvnButt6[EventN];
            EventPicture.BackgroundImage = Image.FromFile("Image/" + EventN + ".png");
            switch (Convert.ToString(EventN))
            {
                case "0":
                    EventButt1.Visible = true;
                    break;
                case "1":
                    if (rFood >= 1)
                    {
                        EventButt1.Visible = true;
                        EventButt3.Visible = true;
                    }
                    if (rWeapon >= 1)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    break;
                case "2":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    break;
                case "3":
                    if (rWeapon >= 1)
                    {
                        EventButt1.Visible = true;
                        EventButt4.Visible = true;
                    }
                    if (rWeapon >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    break;
                case "4":
                    if (rHeal >= 6)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rHeal >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    break;
                case "5":
                    if (rWood >= 6)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rMetal >= 6)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rGold >= 2 && RepTribe >= 250)
                    {
                        EventButt3.Visible = true;
                    }
                    EventButt4.Visible = true;
                    if (RepCastle >= 750)
                    {
                        EventButt5.Visible = true;
                    }
                    break;
                case "6":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (HaveStat[0] == false)
                    {
                        EventButt5.Visible = true;
                    }
                    break;
                case "7":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    if (RepTribe >= 250)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "8":
                    if (rMetal >= 5)
                    {
                        EventButt1.Visible = true;
                    }
                    EventButt2.Visible = true;
                    if (rHeal >= 1)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rWood >= 5)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "9":
                    EventButt1.Visible = true;
                    if (rFood >= 4)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rWood >= 4)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rGold >= 2)
                    {
                        EventButt4.Visible = true;
                    }
                    if (rPeople >= 2)
                    {
                        EventButt5.Visible = true;
                    }
                    if (RepTribe < 250)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "10":
                    if (rWeapon >= 2)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rWeapon >= 1)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (RepTribe >= 750)
                    {
                        EventButt5.Visible = true;
                    }
                    break;
                case "11":
                    if (rWeapon >= 1 && rHeal >= 1 && rPeople >= 3)
                    {
                        EventButt1.Visible = true;
                    }
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    break;
                case "12":
                    EventButt1.Visible = true;
                    if (rHeal >= 2)
                    {
                        EventButt2.Visible = true;
                        EventButt3.Visible = true;
                        EventButt5.Visible = true;
                    }
                    EventButt4.Visible = true;
                    break;
                case "13":
                    if (rMetal >= 4)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rWood >= 4)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rGold >= 2 && RepCity >= 250)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rFood >= 2 && rHeal >= 2 && RepCastle >= 250)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    break;
                case "14":
                    if (rMetal >= 1 && RepCity >= 250 && RepCity < 750)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rPeople >= 3)
                    {
                        EventButt2.Visible = true;
                    }
                    if (RepCastle >= 250 && RepCity < 250)
                    {
                        EventButt3.Visible = true;
                    }
                    EventButt4.Visible = true;
                    if (RepCity >= 750 && rMetal >= 1)
                    {
                        EventButt5.Visible = true;
                    }
                    if (RepCity >= 750)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "15":
                    EventButt1.Visible = true;
                    if (rGold >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rFood >= 4)
                    {
                        EventButt3.Visible = true;
                    }
                    if (RepCastle >= 750)
                    {
                        EventButt4.Visible = true;
                    }
                    if (RepCity >= 750)
                    {
                        EventButt5.Visible = true;
                    }
                    if (rGold >= 4)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "16":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    if (RepTribe >= 250)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rWeapon >= 2)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    break;
                case "17":
                    if (rMetal >= 3)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rWood >= 3)
                    {
                        EventButt2.Visible = true;
                    }
                    if (HaveStat[3] == true)
                    {
                        EventButt3.Visible = true;
                    }
                    EventButt4.Visible = true;
                    if (RepCastle >= 750)
                    {
                        EventButt5.Visible = true;
                    }
                    break;
                case "18":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    if (rWeapon >= 1)
                    {
                        EventButt3.Visible = true;
                    }
                    if (RepCastle >= 750)
                    {
                        EventButt4.Visible = true;
                    }
                    if (RepCastle >= 250 && rGold >= 2)
                    {
                        EventButt5.Visible = true;
                    }
                    if (RepCastle >= 250 && rPeople >= 2)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "19":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (rWeapon >= 1)
                    {
                        EventButt5.Visible = true;
                    }
                    if (rPeople >= 3)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "20":
                    if (rPeople >= 2)
                    {
                        EventButt1.Visible = true;
                        EventButt3.Visible = true;
                    }
                    EventButt2.Visible = true;
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    break;
                case "21":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    break;
                case "22":
                    EventButt1.Visible = true;
                    if (rWeapon >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (HaveStat[5] == true)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rHeal >= 2 && rFood >= 2)
                    {
                        EventButt4.Visible = true;
                    }
                    if (RepCastle >= 750)
                    {
                        EventButt5.Visible = true;
                    }
                    if (rWood >= 2)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "23":
                    EventButt1.Visible = true;
                    if (rMetal >= 2 && rWood >= 1)
                    {
                        EventButt2.Visible = true;
                    }
                    if (RepCastle >= 750)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rPeople >= 2 && rMorale <= 80)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "24":
                    EnemyNum = rnd.Next(10, 13);
                    if (rGold >= 2)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rGold >= 5)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (rGold >= 1 && rMorale <= 90)
                    {
                        EventButt5.Visible = true;
                    }
                    break;
                case "25":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (RepCity >= 750)
                    {
                        EventButt5.Visible = true;
                        EventButt6.Visible = true;
                    }
                    break;
                case "26":
                    //Как же меня заебал этот лагающий ноут. Сука, пока я пытался сделать эту картинку - он завис минут на 10, я ебанусь эту игрушку кодить с такими "мощностями", пытка нахуй
                    EventButt1.Visible = true;
                    if (rHeal >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (RepTribe >= 250)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    EventButt6.Visible = true;
                    break;
                case "27":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    if (rWeapon >= 1)
                    {
                        EventButt3.Visible = true;
                    }
                    if (RepCity >= 250)
                    {
                        EventButt4.Visible = true;
                    }
                    if (rMorale < 30)
                    {
                        EventButt5.Visible = true;
                    }
                    if (RepCastle >= 250)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "28":
                    EventButt1.Visible = true;
                    if (RepCity >= 750)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (rWeapon >= 2)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    break;
                case "29":
                    if (rFood >= 3)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rFood >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    if (RepTribe >= 300)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "30":
                    EventButt1.Visible = true;
                    if (rMetal >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (RepTribe >= 250)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    break;
                case "31":
                    if (rWood >= 4)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rWood >= 3 && rMetal >= 1)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rMetal >= 4)
                    {
                        EventButt3.Visible = true;
                    }
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    if (rGold >= 2 && RepCastle >= 400)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "32":
                    if (rWood >= 3)
                    {
                        EventButt1.Visible = true;
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    break;
                case "33":
                    EventButt1.Visible = true;
                    if (rFood >= 5)
                    {
                        EventButt3.Visible = true;
                        if (rHeal >= 1)
                        {
                            EventButt2.Visible = true;
                        }
                    }
                    if (rMetal >= 5)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "34":
                    EventButt1.Visible = true;
                    if (RepCity >= 250)
                    {
                        EventButt2.Visible = true;
                    }
                    if (RepCastle >= 250)
                    {
                        EventButt3.Visible = true;
                    }
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    EventButt6.Visible = true;
                    break;
                case "35":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    if (rHeal >= 2)
                    { EventButt3.Visible = true; }
                    if (RepCastle >= 250) { EventButt4.Visible = true; }
                    if (RepTribe >= 750) { EventButt5.Visible = true; }
                    break;
                case "36":
                    EventButt1.Visible = true;
                    if (rWood >= 5)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    if (rWood >= 4 && RepCastle >= 250)
                    {
                        EventButt5.Visible = true;
                    }
                    if (RepTribe >= 750 && RepCastle < 250)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "37":
                    EventButt1.Visible = true;
                    if (RepCity >= 350)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (RepTribe >= 250 && rGold < 5)
                    {
                        EventButt4.Visible = true;
                    }
                    if (rPeople >= 3)
                    {
                        EventButt5.Visible = true;
                    }    
                    break;
                case "800":
                    EventButt1.Visible = true;
                    if (rWood >= 3)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rHeal >= rPeople)
                    {
                        EventButt3.Visible = true;
                    }
                    if (rPeople >= 2)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "850":
                    EnemyNum = rnd.Next(2, 13);
                    EventText.Text += "" + EnemyNum;
                    EventButt1.Visible = true;
                    if (rWood >= 2 || rMetal >= 2 || rHeal >= 2 || rFood >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rGold >= 6 && RepTribe != 0)
                    {
                        EventButt3.Visible = true;
                    }
                    break;
                case "851":
                    EnemyNum = rnd.Next(2, 13);
                    EventText.Text += "" + EnemyNum;
                    EventButt1.Visible = true;
                    if (rWood >= 2 || rMetal >= 2 || rHeal >= 2 || rFood >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rGold >= 6 && RepCastle != 0)
                    {
                        EventButt3.Visible = true;
                    }
                    break;
                case "852":
                    EnemyNum = rnd.Next(2, 13);
                    EventText.Text += "" + EnemyNum;
                    EventButt1.Visible = true;
                    if (rWood >= 2 || rMetal >= 2 || rHeal >= 2 || rFood >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rGold >= 6 && RepCity != 0)
                    {
                        EventButt3.Visible = true;
                    }
                    break;
                case "853":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    break;
                case "854":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    EventButt3.Visible = true;
                    if (rWood >= 8)
                    {
                        EventButt4.Visible = true;
                    }
                    break;
                case "855":
                    EventButt1.Visible = true;
                    if (rHeal >= 6)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (RepCity >= 750 && rGold >= 1)
                    {
                        EventButt4.Visible = true;
                    }    
                    break;
                case "856":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    break;
                case "857":
                    if (RepTribe >= 250 && RepCastle >= 250 && RepCity >= 250 && RepTribe < 750 && RepCastle < 750 && RepCity < 750)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rGold >= 4)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    EventButt4.Visible = true;
                    EventButt5.Visible = true;
                    EventButt6.Visible = true;
                    break;
                case "858":
                    if (rPeople >= 4)
                    {
                        EventButt1.Visible = true;
                    }
                    EventButt2.Visible = true;
                    break;
                case "900":
                    if (rPeople >= rFood)
                    {
                        EventButt1.Visible = true;
                    }
                    if (rPeople >= 2 && rWeapon >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    EventButt3.Visible = true;
                    if (rPeople >= rGold)
                    {
                        EventButt4.Visible = true;
                    }
                    EventButt5.Visible = true;
                    break;
                case "901":
                    EventButt1.Visible = true;
                    if (rWeapon >= (rPeople / 2) && rPeople >= 2)
                    {
                        EventButt2.Visible = true;
                    }
                    if (rHeal >= 1 || rWood >= 1 || rMetal >= 1)
                    {
                        EventButt3.Visible = true;
                    }
                    break;
                case "902":
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    if (rPeople >= 2)
                    {
                        EventButt3.Visible = true;
                    }    
                    if (rWood >= 10)
                    {
                        EventButt4.Visible = true;
                    }
                    if (rHeal >= rPeople)
                    {
                        EventButt5.Visible = true;
                    }
                    if (rMetal >= 6 && rGold >= 6)
                    {
                        EventButt6.Visible = true;
                    }
                    break;
                case "999":
                    EventText.Text += R + Word[52 + T] + P + Word[59 + B]; //T - айди покупки, B - айди продажи
                    EventButt1.Visible = true;
                    EventButt2.Visible = true;
                    break;
                case "1000":
                    EventText.Text = "Сегодня вы обнаружили остатки одного исследователя индустриального мира. У него была карта округи, на которой\nкрестиком была обозначена одна территория. Неужели тут где то закопаны драгоценности?";
                    EventButt1.Visible = true;
                    EventButt1.Text = "Соберите общину, мы идём искать золото!";
                    EventButt2.Visible = true;
                    EventButt2.Text = "Лучше бы ты сухих веток принёс, это долго гореть не будет...";
                    break;
                case "1001":
                    EventText.Text = "Вы отправились в путь с несколькими членами племени. Вашим первым препятствием стал тот факт, что на пути \nк кладу развернулась огромная водная гладь, похоже, это озеро, но на карте тут вообще не должно быть никакой\nводы.";
                    EventButt1.Visible = true;
                    EventButt1.Text = "Давайте вернёмся и убедимся, что мы туда идём";
                    if (rWood >= 3)
                    {
                        EventButt2.Visible = true;
                        EventButt2.Text = "Быстро сделайте плот и поплыли напрямик";
                    }
                    EventButt3.Visible = true;
                    EventButt3.Text = "Мы должны поискать обход";
                    break;
                case "1002":
                    EventText.Text = "Кажется, пытаясь найти правильную дорогу к кладу мы малость заплутали и вышли на незнакомую нам местность. \nТем временем, приближается ночь...";
                    EventButt1.Visible = true;
                    EventButt1.Text = "Переждём ночь на открытом воздухе, не в первой";
                    EventButt2.Visible = true;
                    EventButt2.Text = "К чёрту всё это! Ищите дорогу домой пока нас не сожрали";
                    EventButt3.Visible = true;
                    EventButt3.Text = "Продолжаем искать путь к кладу, даже ночью";
                    break;
                case "1003": //Они обошли
                    break;
                case "1004": //Они рвутся домой
                    break;
                case "1005": //Они рвутся дальше
                    break;
            }
            CheckRes();
        }

        private void EventButt1_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "0":
                    break;
                case "1":
                    rPeople++; 
                    rMorale -= 3; 
                    RepCastle -= rnd.Next(10, 31);
                    break;
                case "2":
                    int N = rnd.Next(1, 5);
                    if ( rHeal >= N) { rHeal -= N; } else { U = rnd.Next(1, 3); rPeople -= U; rMorale -= 4;  }
                    Y = rnd.Next(1, 5); rMetal += Y; 
                    break;
                case "3":
                    N = rnd.Next(3, 6);
                    rFood += N; 
                    break;
                case "4":
                    rHeal -= 6; 
                    rMorale += 2;
                    break;
                case "5":
                    rWood -= 6;
                    break;
                case "6":
                    N = rnd.Next(8, 11);
                    rFood += N; 
                    break;
                case "7":
                    rMorale += 5;
                    rPeople -= 1; 
                    break;
                case "8":
                    N = rnd.Next(1, 21);
                    if (N == 20) { rGold += 5; }
                    rMetal -= 5; 
                    break;
                case "9":
                    rMorale -= 20; 
                    break;
                case "10":
                    N = rnd.Next(6, 13);
                    rFood += N; 
                    rWood++; 
                    rWeapon -= 2; 
                    break;
                case "11":
                    N = rnd.Next(1, 6);
                    if (N == 3) { rMetal += 2; }
                    if (N == 4) { rWood += 2;}
                    rWeapon -= 1;
                    rHeal -= 1; 
                    break;
                case "12":
                    rFood++; 
                    break;
                case "13":
                    rMetal -= 4; 
                    rWeapon += 4;
                    break;
                case "14":
                    rMetal--; 
                    rGold += 8; 
                    RepCity -= 125;
                    break;
                case "15":
                    RepCastle -= 50;
                    rMorale += 5; 
                    break;
                case "16":
                    EnemyDevLVL = 0;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    EnemyNum = rnd.Next(2, 6);
                    BattleType = false;
                    Battle();
                    break;
                case "17":
                    rMetal -= 3; 
                    rMorale -= 5; 
                    rFood += 3;
                    break;
                case "18":
                    N = rnd.Next(2, 6);
                    rFood += 5; 
                    break;
                case "19":
                    EnemyDevLVL = 0;
                    EnemyNum = rnd.Next(6, 11);
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = 3;
                    N = rnd.Next(3, 10);
                    rFood += N; 
                    N = rnd.Next(2, 7);
                    rMetal += N;
                    N = rnd.Next(1, 2);
                    rGold += N; 
                    N = rnd.Next(1, 5);
                    rHeal += N; 
                    rMorale += 10;
                    RepTribe -= 100;
                    BattleType = false;
                    Battle(); 
                    break;
                case "20":
                    rPeople--;
                    RepCity += 30; 
                    break;
                case "21":
                    N = rnd.Next(1, 4);
                    if (N != 3) { rPeople += 3;  } else { rPeople++;  }
                    break;
                case "22":
                    HaveStat[3] = false;
                    rMorale -= 10; 
                    rFood--; 
                    break;
                case "23":
                    rMorale++; 
                    rWood = 0;
                    rMetal = 0;
                    break;
                case "24":
                    N = rnd.Next(1, 9);
                    if (N == 8) { rGold += 6; } else { rGold -= 2; }
                    break;
                case "25":
                    rMorale -= 12;
                    RepCity += 40;
                    break;
                case "26":
                    N = rnd.Next(1, 3);
                    if (rHeal <= 1) { rPeople -= N; }
                    rHeal -= 2;
                    rFood += 5;
                    break;
                case "27":
                    rMorale -= 10;
                    RepCity += 10;
                    break;
                case "28":
                    rFood += rnd.Next(1,3);
                    break;
                case "29":
                    rPeople += 2;
                    rFood -= 2;
                    rMorale -= 4;
                    break;
                case "30":
                    rWeapon++;
                    break;
                case "31":
                    rWood -= 4;
                    rWeapon += 3;
                    break;
                case "32":
                    rWood -= 4;
                    rGold++;
                    RepCity += 75;
                    break;
                case "33":
                    rMorale++;
                    rFood++;
                    break;
                case "34":
                    rFood++;
                    rMorale += 2;
                    break;
                case "35":
                    rPeople--;
                    if (rHeal >= 1) { rHeal--; } else { rPeople--; }
                    rFood--;
                    break;
                case "36":
                    rWood += 2;
                    rMetal += 3;
                    rWeapon += 1;
                    rFood += 4;
                    rHeal += 3;
                    rMorale += 15;
                    RepCastle -= 100;
                    EnemyDevLVL = 1;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(2);
                    EnemyNum = rnd.Next(3, 7);
                    BattleType = false;
                    Battle();
                    break;
                case "37":
                    rPeople++;
                    rMorale -= 20;
                    break;
                case "800":
                    rMorale -= 10; 
                    break;
                case "850":
                    EnemyDevLVL = 0;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    BattleType = true;
                    Battle();
                    break;
                case "851":
                    EnemyDevLVL = 1;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    BattleType = true;
                    Battle();
                    break;
                case "852":
                    EnemyDevLVL = 2;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    BattleType = true;
                    Battle();
                    break;
                case "853":
                    HaveStat[1] = true;
                    BonStatus.Text = Word[5] + StatText[1];
                    rGold -= 4; 
                    break;
                case "854":
                    HaveStat[3] = true;
                    BonStatus.Text = Word[5] + StatText[3];
                    rGold -= 4; 
                    break;
                case "855":
                    HaveStat[2] = true;
                    BonStatus.Text = Word[5] + StatText[2];
                    rMorale += 6;
                    break;
                case "856":
                    HaveStat[5] = true;
                    BonStatus.Text = Word[5] + StatText[5];
                    rWood -= 12; 
                    break;
                case "857":
                    RepTribe += rnd.Next(-15, 15);
                    RepCastle += rnd.Next(-15, 15);
                    RepCity += rnd.Next(-15, 15);
                    if (rnd.Next(1,5) == 4)
                    {
                        HaveStat[6] = true;
                        BonStatus.Text = Word[5] + StatText[6];
                    }
                    rMorale++;
                    break;
                case "858":
                    HaveStat[7] = true;
                    BonStatus.Text = Word[5] + StatText[7];
                    rPeople -= 3;
                    break;
                case "900":
                    rFood -= rPeople;
                    rMorale += 25; 
                    break;
                case "901":
                    rMorale -= 10;
                    break;
                case "902":
                    rMorale -= 10;
                    break;
                case "999":
                    TradeTrack.Visible = false;
                    switch (Convert.ToString(T))
                    {
                        case "1":
                            rPeople += TradeTrack.Value;
                            break;
                        case "2":
                            rFood += TradeTrack.Value * R;
                            break;
                        case "3":
                            rHeal += TradeTrack.Value * R;
                            break;
                        case "4":
                            rWood += TradeTrack.Value * R;
                            break;
                        case "5":
                            rMetal += TradeTrack.Value * R;
                            break;
                        case "6":
                            rWeapon += TradeTrack.Value * R;
                            break;
                        case "7":
                            rGold += TradeTrack.Value * R;
                            break;
                    }
                    switch (Convert.ToString(B))
                    {
                        case "1":
                            rPeople -= TradeTrack.Value;
                            break;
                        case "2":
                            rFood -= TradeTrack.Value * P;
                            break;
                        case "3":
                            rHeal -= TradeTrack.Value * P;
                            break;
                        case "4":
                            rWood -= TradeTrack.Value * P;
                            break;
                        case "5":
                            rMetal -= TradeTrack.Value * P;
                            break;
                        case "6":
                            rWeapon -= TradeTrack.Value * P;
                            break;
                        case "7":
                            rGold -= TradeTrack.Value * P;
                            break;
                    }
                    TradeTrack.Value = 0;
                    break;
                case "1000":
                    EventN = 1001;
                    EventU = true;
                    break;
                case "1001":
                    EventN = 1002;
                    break;
                case "1002":
                    EventN = 1004;
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        private void EventButt2_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "1":
                    rMorale += 3; 
                    break;
                case "2":
                    rMetal++; 
                    break;
                case "3":
                    rWeapon -= 2; 
                    N = rnd.Next(6, 10);
                    rFood += N; 
                    break;
                case "4":
                    rHeal -= 2; 
                    N = rnd.Next(1, 5);
                    if (N != 4) { rPeople--;  }
                    break;
                case "5":
                    rMetal -= 6;
                    rMorale += 3;
                    break;
                case "6":
                    N = rnd.Next(1, 3);
                    U = rnd.Next(2, 4);
                    rFood += N; 
                    rHeal += U; 
                    break;
                case "7":
                    N = rnd.Next(1, 6); if (N == 5) { rPeople -= 1; rGold = 0; }
                    rMorale -= 12; 
                    break;
                case "9":
                    rFood -= 4; 
                    break;
                case "10":
                    N = rnd.Next(5, 12);
                    Y = rnd.Next(1, 4);
                    if (Y == 3 && rHeal <= 1) { rPeople--; } if (Y == 3 && rHeal >= 1) { rHeal--;  }
                    rFood += N; 
                    rWood++; 
                    rWeapon -= 1; 
                    break;
                case "11":
                    rPeople -= 1; 
                    break;
                case "12":
                    rHeal -= 2; 
                    rPeople++;
                    break;
                case "13":
                    rWood -= 4; 
                    rWeapon += 2; 
                    break;
                case "14":
                    rPeople -= 2; 
                    rGold += 4;
                    rMorale -= 7; 
                    RepCity += 20;
                    break;
                case "15":
                    rGold -= 2; 
                    break;
                case "16":
                    rMorale -= 12; 
                    break;
                case "17":
                    rFood += 3; 
                    rWood -= 3; 
                    break;
                case "18":
                    N = rnd.Next(1, 5);
                    RepCastle -= 50 * N;
                    rPeople += N; 
                    break;
                case "19":
                    EnemyDevLVL = 1;
                    EnemyNum = rnd.Next(6, 11);
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = 3;
                    N = rnd.Next(5, 13);
                    rFood += N; 
                    N = rnd.Next(3, 9);
                    rMetal += N; 
                    N = rnd.Next(2, 3);
                    rGold += N; 
                    N = rnd.Next(2, 7);
                    rHeal += N; 
                    rMorale += 10; 
                    RepCastle -= 100;
                    BattleType = false;
                    Battle();
                    break;
                case "21":
                    EnemyNum = rnd.Next(1, 4);
                    if (N == 3) { rPeople += 3;  }
                    if (N == 2) { rPeople++;  }
                    rMorale += 10; 
                    break;
                case "22":
                    rWeapon -= 2; 
                    rMorale += 5; 
                    rFood++; 
                    N = rnd.Next(1, 4);
                    Y = rnd.Next(1, 3);
                    if (N == 3 && rHeal < Y) { rPeople--;  }
                    if (N == 3 && rHeal >= Y) { rHeal -= Y; }
                    break;
                case "23":
                    rMetal -= 2; 
                    rWood--; 
                    rMorale += 3; 
                    break;
                case "24":
                    N = rnd.Next(1, 9);
                    if (N == 8) { rGold += 15; } else { rGold -= 5; }
                    break;
                case "25":
                    rMorale -= 8;
                    rGold += 1;
                    break;
                case "26":
                    N = rnd.Next(5, 8);
                    rHeal -= 2;
                    rFood += N;
                    rMorale--;
                    break;
                case "27":
                    if ( rnd.Next(1,11) > 5)
                    {
                        rMorale -= 5;
                    }
                    else
                    {
                        rMorale -= 15;
                    }
                    break;
                case "28":
                    rFood += 9;
                    RepCity -= 15;
                    break;
                case "29":
                    rPeople += 1;
                    rFood -= 1;
                    rMorale -= 2;
                    break;
                case "30":
                    rMetal -= rnd.Next(1, 3);
                    if (rnd.Next(1,4) == 2)
                    {
                        rWeapon += 6;
                    }
                    else
                    {
                        rWeapon++;
                    }
                    break;
                case "31":
                    rWood -= 3;
                    rMetal--;
                    rWeapon += 4;
                    break;
                case "32":
                    rWood -= 4;
                    rFood += 4;
                    RepCity += 75;
                    break;
                case "33":
                    rFood -= 5;
                    rHeal -= 1;
                    if (rnd.Next(1,16) == 4 )
                    {
                        rPeople++;
                        rMorale += 20;
                    }
                    break;
                case "34":
                    rGold += 2;
                    rMorale -= 5;
                    break;
                case "35":
                    rFood -= 3;
                    break;
                case "36":
                    rWood -= 5;
                    RepCastle += 150;
                    break;
                case "37":
                    RepCity += 30;
                    rMorale -= 10;
                    break;
                case "800":
                    rWood -= 3; 
                    rFood++; 
                    break;
                case "850":
                    rWood = 0; rHeal = 0; rMetal = 0; rFood /= 2;
                    break;
                case "851":
                    rWood = 0; rHeal = 0; rMetal = 0; rFood /= 2;
                    break;
                case "852":
                    rWood = 0; rHeal = 0; rMetal = 0; rFood /= 2;
                    break;
                case "853":
                    HaveStat[1] = true;
                    BonStatus.Text = Word[5] + StatText[1];
                    rGold -= 3; 
                    RepCity -= rnd.Next(21, 60);
                    break;
                case "854":
                    EnemyDevLVL = 0;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    HaveStat[3] = true;
                    BonStatus.Text = Word[5] + StatText[3];
                    EnemyNum = rnd.Next(5, 10);
                    RepTribe -= 250;
                    rMorale += 10; 
                    rFood += 3;
                    BattleType = false;
                    Battle();
                    break;
                case "855":
                    rHeal -= 6;
                    N = rnd.Next(1, 11); if (N == 5) { rPeople -= 1; HaveStat[2] = true; BonStatus.Text = Word[5] + StatText[2]; }
                    break;
                case "857":
                    rGold -= 4;
                    break;
                case "900":
                    rWeapon -= 2;
                    rPeople--; 
                    rMorale += 12; 
                    break;
                case "901":
                    rMorale = 50;
                    N = rnd.Next(1, (rPeople));
                    rPeople -= N; 
                    rWeapon -= N / 2; 
                    break;
                case "902":
                    N = rnd.Next(1, 11);
                    if (N == 10) { rMorale += 20; } else { rMorale = 0; rPeople--; }
                    break;
                case "999":
                    TradeTrack.Visible = false;
                    break;
                case "1000":
                    EventU = false;
                    break;
                case "1001":
                    EventN = 1003;
                    rWood -= 3;
                    break;
                case "1002":
                    EventN = 1004;
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        private void EventButt3_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "1":
                    rMorale--;  
                    rWood++;  
                    break;
                case "2":
                    rWood++;  
                    break;
                case "3":
                    N = rnd.Next(1, 3);
                    if (N == 2) { rHeal++;  }
                    if (N == 1) { rWood++;  }
                    rFood++; 
                    break;
                case "4":
                    rPeople--; 
                    rMorale -= 3; 
                    break;
                case "5":
                    rGold -= 2;
                    RepTribe += 25;
                    break;
                case "6":
                    rFood += 1; 
                    rMorale += 20; 
                    break;
                case "7":
                    N = rnd.Next(1, 3);
                    if (N == 2) { rMorale += 2;  }
                    rPeople--; 
                    rFood++; 
                    break;
                case "8":
                    rHeal--; 
                    rMorale += 2;
                    break;
                case "9":
                    rWood -= 4; 
                    break;
                case "10":
                    N = rnd.Next(3, 10);
                    Y = rnd.Next(1, 4);
                    if (Y == 3 && rHeal <= 4) { rPeople -= 2;  }
                    if (Y == 3 && rHeal >= 4) { rHeal -= 4;  }
                    rFood += N; 
                    break;
                case "11":
                    N = rnd.Next(1, 4);
                    if (N != 3) { rPeople -= 1; rMorale -= 1; } else { rMorale += 4; }
                    break;
                case "12":
                    rHeal -= 2; 
                    rWeapon += 3;
                    break;
                case "13":
                    rGold -= 2;
                    rWeapon += 3;
                    RepCity += 10;
                    break;
                case "14":
                    RepCastle += 25;
                    RepCity -= 10;
                    rGold++; 
                    break;
                case "15":
                    rFood -= 4; 
                    break;
                case "16":
                    rMorale += 3; 
                    RepTribe -= 33;
                    break;
                case "17":
                    N = rnd.Next(7, 13);
                    HaveStat[3] = false;
                    rFood += N; 
                    break;
                case "18":
                    N = rnd.Next(5, 13);
                    rWeapon--; 
                    rFood += N; 
                    RepCastle -= 65;
                    break;
                case "19":
                    EnemyDevLVL = 2;
                    EnemyNum = rnd.Next(6, 11);
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = 3;
                    N = rnd.Next(8, 17);
                    rFood += N; 
                    N = rnd.Next(6, 11);
                    rMetal += N; 
                    N = rnd.Next(3, 5);
                    rGold += N; 
                    N = rnd.Next(4, 9);
                    rHeal += N; 
                    rMorale += 10; 
                    RepCity -= 100;
                    BattleType = false;
                    Battle();
                    break;
                case "20":
                    rPeople--; 
                    N = rnd.Next(1, 3);
                    rGold += N; 
                    break;
                case "21":
                    rPeople += 3; 
                    rMorale -= 10; 
                    break;
                case "22":
                    rMorale += 3; 
                    break;
                case "23":
                    RepCastle -= 30;
                    break;
                case "24":
                    RepCity -= 75;
                    EnemyDevLVL = 2;
                    rGold += 12;
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    BattleType = false;
                    Battle(); 
                    break;
                case "25":
                    rMorale += 12;
                    RepCity -= 40;
                    break;
                case "27":
                    rWeapon--;
                    rMetal += rnd.Next(1, 4);
                    RepCity -= rnd.Next(10, 21);
                    break;
                case "30":
                    rMetal += rnd.Next(1,3);
                    break;
                case "31":
                    rMetal -= 4;
                    rWeapon += 5;
                    break;
                case "32":
                    rMorale += 3;
                    RepCity -= 10;
                    break;
                case "33":
                    rFood -= 5;
                    if (rnd.Next(1, 20) == 4)
                    {
                        rPeople++;
                        rMorale += 20;
                    }
                    break;
                case "34":
                    rGold += 1;
                    RepCastle += 30;
                    rMorale -= 5;
                    break;
                case "35":
                    rHeal -= 2;
                    if (rnd.Next(1,3) == 2)
                    {
                        rFood--;
                    }
                    if (rnd.Next(1, 3) == 2)
                    {
                        rFood--;
                    }
                    break;
                case "36":
                    rFood += rnd.Next(0, 3);
                    rWood++;
                    if (rnd.Next(1,3) == 2)
                    {
                        rMetal++;
                    }
                    RepCastle -= 50;
                    break;
                case "37":
                    rHeal++;
                    rFood++;
                    rMorale += 5;
                    RepCity -= 10;
                    break;
                case "800":
                    rFood += rPeople; 
                    rHeal -= rPeople; 
                    rMorale += 3; 
                    break;
                case "850":
                    rGold -= 6;
                    break;
                case "851":
                    rGold -= 6;
                    break;
                case "852":
                    rGold -= 6;
                    break;
                case "855":
                    N = rnd.Next(1, 5); if (N == 3) { rPeople -= 1;  HaveStat[2] = true; BonStatus.Text = Word[5] + StatText[2]; }
                    break;
                case "857":
                    RepTribe -= rnd.Next(0, 10);
                    RepCastle -= rnd.Next(0, 10);
                    RepCity -= rnd.Next(0, 10);
                    rWeapon += rnd.Next(1, 5);
                    HaveStat[6] = true;
                    BonStatus.Text = Word[5] + StatText[6];
                    break;
                case "900":
                    rMorale -= 8; 
                    break;
                case "901":
                    rMorale = 40; 
                    if (rHeal >= 2) { N = rnd.Next(1, rHeal); rHeal -= N;  }
                    if (rWood >= 2) { N = rnd.Next(1, rWood); rWood -= N;  }
                    if (rMetal >= 2) { N = rnd.Next(1, rMetal); rMetal -= N;  }
                    break;
                case "902":
                    N = rnd.Next(1, 3);
                    if (N == 2) { rMorale += 10;  }
                    rPeople--; 
                    break;
                case "1001":
                    rFood -= 2;
                    EventN = 1003;
                    break;
                case "1002":
                    EventN = 1005;
                    rMorale -= 5; 
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        private void EventButt4_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "1":
                    rFood++; 
                    break;
                case "3":
                    rMorale += 5; 
                    rFood++; 
                    break;
                case "5":
                    rMorale -= 12;
                    break;
                case "6":
                    rFood += 2;
                    RepTribe += rnd.Next(20, 51);
                    break;
                case "7":
                    rPeople--; 
                    rGold++;
                    break;
                case "8":
                    N = rnd.Next(1, 101);
                    if (N == 20) { rGold += 5; }
                    rWood -= 5; 
                    break;
                case "9":
                    rGold -= 2; 
                    break;
                case "12":
                    N = rnd.Next(1, 6);
                    if (N == 5) { rPeople--; rWeapon += 1; } else { rWeapon += 3;  }
                    RepCity -= 20;
                    rMorale--; 
                    break;
                case "13":
                    rFood -= 2; 
                    rHeal -= 2; 
                    rWeapon += 2; 
                    RepCastle += 10;
                    break;
                case "15":
                    RepCastle -= 25;
                    break;
                case "16":
                    rWeapon -= 2;
                    break;
                case "17":
                    rMorale -= 5; 
                    break;
                case "18":
                    RepCastle -= 20;
                    rFood += 8; 
                    break;
                case "19":
                    rMorale -= 10; 
                    RepCastle++;
                    RepCity++;
                    RepTribe++;
                    break;
                case "20":
                    N = rnd.Next(3, 8);
                    rFood += N; 
                    rMetal++; 
                    RepCity -= 40;
                    break;
                case "22":
                    rHeal -= 2; 
                    rFood -= 1; 
                    break;
                case "23":
                    rWood = 0;
                    rMetal = 0;
                    rPeople--; 
                    rMorale += 40; 
                    break;
                case "25":
                    rMorale += 15;
                    rFood += 4;
                    RepCity -= 75;
                    break;
                case "26":
                    RepTribe -= 50;
                    rGold += 1;
                    break;
                case "27":
                    rMorale++;
                    RepCity--;
                    break;
                case "28":
                    rWeapon--;
                    rFood += 9;
                    RepCity -= 50;
                    break;
                case "29":
                    if (rnd.Next(1,3) == 2)
                    {
                        RepCity -= 20;
                    }
                    else
                    {
                        RepCastle -= 20;
                    }
                    rFood += 3;
                    rMorale += 2;
                    break;
                case "30":
                    rGold++;
                    RepTribe += 10;
                    break;
                case "31":
                    rMorale -= 5;
                    break;
                case "33":
                    rMetal -= 5;
                    if (rnd.Next(1, 20) == 4)
                    {
                        rPeople++;
                        rWeapon++;
                        rMorale += 40;
                    }
                    break;
                case "34":
                    RepTribe += 50;
                    rMorale -= 5;
                    break;
                case "35":
                    RepCastle -= 50;
                    rFood -= 4;
                    rGold++;
                    rMorale += 3;
                    break;
                case "36":
                    rWood += rnd.Next(1, 3);
                    break;
                case "37":
                    RepTribe += 30;
                    rGold += 1;
                    rMorale -= 20;
                    break;
                case "800":
                    rPeople--; 
                    rFood++; 
                    break;
                case "854":
                    HaveStat[3] = true;
                    BonStatus.Text = Word[5] + StatText[3];
                    rWood -= 8; 
                    break;
                case "855":
                    RepCity -= 25;
                    rGold--; 
                    break;
                case "857":
                    if (rnd.Next(0,5) != 3)
                    {
                        HaveStat[6] = true;
                        BonStatus.Text = Word[5] + StatText[6];
                    }
                    break;
                case "900":
                    rGold -= rPeople; 
                    rMorale += 30; 
                    break;
                case "902":
                    rWood -= 10; 
                    rMorale += 12;
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        private void EventButt5_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "1":
                    RepCastle += 15;
                    break;
                case "5":
                    RepCastle -= 25;
                    break;
                case "6":
                    HaveStat[0] = true;
                    BonStatus.Text = Word[5] + StatText[0];
                    break;
                case "9":
                    rPeople -= 1; 
                    break;
                case "10":
                    N = rnd.Next(3, 7);
                    rFood += N; 
                    break;
                case "12":
                    rHeal -= 2; 
                    RepCity += 25;
                    break;
                case "13":
                    rFood++; 
                    break;
                case "14":
                    rMetal--; 
                    rGold += 8; 
                    RepCity -= 50;
                    break;
                case "15":
                    RepCity -= 10;
                    RepCastle -= 10;
                    break;
                case "16":
                    rMorale -= 18; 
                    RepTribe += 30;
                    break;
                case "17":
                    RepCastle -= 30;
                    rFood += 5;
                    break;
                case "18":
                    rFood += 15; 
                    rGold -= 2; 
                    RepCastle += 12;
                    break;
                case "19":
                    rWeapon--; 
                    N = rnd.Next(3, 8);
                    rFood += N; 
                    rMorale -= 3; 
                    break;
                case "20":
                    RepCity -= 20;
                    rMetal++; 
                    rHeal++; 
                    break;
                case "22":
                    RepCastle -= 30;
                    break;
                case "24":
                    rGold--;
                    rMorale += 10;
                    break;
                case "25":
                    rMorale += 3;
                    RepCity -= 15;
                    rGold += 1;
                    break;
                case "26":
                    N = rnd.Next(1, 3);
                    rMorale += N;
                    break;
                case "27":
                    rMorale += 5;
                    RepCity += 10;
                    break;
                case "28":
                    rHeal++;
                    break;
                case "29":
                    if (rnd.Next(1, 3) == 2)
                    {
                        RepCity -= 20;
                    }
                    else
                    {
                        RepCastle -= 20;
                    }
                    rWood += 2;
                    rMetal += 1;
                    break;
                case "30":
                    rFood += rnd.Next(1, 3);
                    break;
                case "31":
                    rMorale -= 8;
                    rWeapon += 1;
                    break;
                case "34":
                    rWood++;
                    rMorale += 8;
                    break;
                case "35":
                    RepTribe -= 50;
                    rMorale++;
                    break;
                case "36":
                    rWood -= 4;
                    rGold += 2;
                    rFood += 2;
                    break;
                case "37":
                    rMorale += 15;
                    rPeople--;
                    break;
                case "857":
                    HaveStat[6] = true;
                    BonStatus.Text = Word[5] + StatText[6];
                    rMorale += 10;
                    break;
                case "900":
                    N = rnd.Next(1, 7);
                    if (N == 6) { rMorale += 8; } else { rMorale -= 15; }
                    break;
                case "902":
                    rHeal -= rPeople; 
                    rMorale += rPeople + 3; 
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        private void EventButt6_Click(object sender, EventArgs e)
        {
            ShutDownString();
            SetRes();
            switch (Convert.ToString(EventN))
            {
                case "9":
                    EnemyDevLVL = 0;
                    RepTribe -= 50;
                    EnemyNum = rnd.Next(2, 7);
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    BattleType = false;
                    Battle(); 
                    break;
                case "14":
                    rGold += 3; 
                    RepCity -= 30;
                    break;
                case "15":
                    rGold -= 4; 
                    RepCastle += 30;
                    break;
                case "18":
                    rPeople--; 
                    rGold += 2; 
                    RepCastle += 15;
                    break;
                case "19":
                    rMorale += 6; 
                    if (rHeal <= 3)
                    {
                        N = rnd.Next(1, 3);
                        rPeople -= N; 
                        rHeal = 0; 
                    }
                    else
                    {
                        rHeal -= 3; 
                        N = rnd.Next(1, 13);
                        if (N == 12) { rPeople--; }
                    }
                    break;
                case "22":
                    rWood -= 2; 
                    rMorale += 5;
                    break;
                case "25":
                    rWood += 2;
                    rFood += 2;
                    rMorale += 3;
                    RepCity -= 15;
                    break;
                case "26":
                    N = rnd.Next(1, 3);
                    rWood += N;
                    break;
                case "27":
                    RepCastle += 20;
                    RepCity -= 30;
                    rMorale += 10;
                    break;
                case "29":
                    if (rnd.Next(1, 3) == 2)
                    {
                        RepCity -= 20;
                    }
                    else
                    {
                        RepCastle -= 20;
                    }
                    RepTribe += 10;
                    rGold++;
                    break;
                case "31":
                    rGold -= 2;
                    RepCastle += 10;
                    rWeapon += 5;
                    break;
                case "36":
                    if (rWeapon >= 1)
                        rWeapon--;
                    else
                    {
                        if (rHeal >= 1)
                            rHeal--;
                        else
                            rPeople--;
                    }
                    RepCastle -= 150;
                    RepTribe += 50;
                    rWood++;
                    rMetal += 2;
                    rWeapon += 1;
                    rFood += 2;
                    rHeal += 1;
                    rMorale += 7;
                    break;
                case "857":
                    EnemyDevLVL = Convert.ToByte(rnd.Next(0,3));
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    EnemyNum = rnd.Next(2, 6);
                    BattleType = false;
                    Battle();
                    break;
                case "902":
                    rMetal -= 6; 
                    rGold -= 6; 
                    rMorale += 35; 
                    break;
            }
            if (BattleGo == false)
            {
                EventAlgoritm();
                EventLoad();
            }
            BonusSee();
            BonusShow.Start();
        }

        public void GG()
        {
            HideGameplay();
            NumDay.Visible = false;
            SaveGame.Visible = false;
            NumFood.Visible = false;
            NumGold.Visible = false;
            NumHeal.Visible = false;
            NumMetal.Visible = false;
            NumMorale.Visible = false;
            NumPeople.Visible = false;
            NumWeapon.Visible = false;
            NumWoodStone.Visible = false;
            BonHeal.Visible = false;
            BonGold.Visible = false;
            BonFood.Visible = false;
            BonMetal.Visible = false;
            BonMorale.Visible = false;
            BonPeople.Visible = false;
            BonWeapon.Visible = false;
            BonWood.Visible = false;
            ButtStat.Visible = false;
            EventText.Text = Word[4];
            EventPicture.BackgroundImage = Image.FromFile("Image/GG.png");
            EventPicture.Visible = true;
            EventText.Visible = true;
            Butt[1] = new Button
            {
                Width = 120,
                Height = 40,
                Location = new Point(200, 300),
                Name = "Restart",
                Text = Word[22]
            };
            this.Butt[1].Click += new EventHandler(RestartGame);
            this.Controls.Add(Butt[1]);
            Butt[1].Visible = true;
        }

        public void Win()
        {
            HideGameplay();
            NumDay.Visible = false;
            SaveGame.Visible = false;
            NumFood.Visible = false;
            NumGold.Visible = false;
            NumHeal.Visible = false;
            NumMetal.Visible = false;
            NumMorale.Visible = false;
            NumPeople.Visible = false;
            NumWeapon.Visible = false;
            NumWoodStone.Visible = false;
            BonHeal.Visible = false;
            BonGold.Visible = false;
            BonFood.Visible = false;
            BonStatus.Visible = false;
            BonMetal.Visible = false;
            BonMorale.Visible = false;
            BonPeople.Visible = false;
            BonWeapon.Visible = false;
            BonWood.Visible = false;
            EventText.Text = Word[3];
            EventPicture.Visible = true;
            EventText.Visible = true;
        }

        private void Battle()
        {
            BattleText.Text = "";
            BattleTacText.Text = Word[21];
            PanelBack = new PictureBox
            {
                Width = 800,
                Height = 150,
                Location = new Point(0, 0),
                Name = "PanelBack",
                BackColor = Color.Transparent,
            };
            PanelPic.Controls.Add(PanelBack);


            BattleRetrate.Text = Word[11];
            BattleRetrate.Visible = true;
            PanelBattle.Visible = true;
            PanelBattle.Location = new Point(0, 0);
            if (BattleType == true)
            {
                BattleTypeText.Text = Word[8];
                BattleRetrate.Visible = false;
            }
            else
            {
                BattleTypeText.Text = Word[9];
                BattleRetrate.Visible = true;
            }
            BattleGo = true; //Типо, идёт битва или нет?
            LaberYouMan.Text = Word[102] + rPeople;
            LaberEnemyMan.Text = Word[103] + EnemyNum;
            switch (Convert.ToString(DevLVL))
            {
                case "0":
                    YouDev.Text = Word[72];
                    break;
                case "1":
                    YouDev.Text = Word[73];
                    break;
                case "2":
                    YouDev.Text = Word[74];
                    break;
                case "3":
                    YouDev.Text = Word[75];
                    break;
            }
            switch (Convert.ToString(EnemyDevLVL))
            {
                case "0":
                    EnemyDev.Text = Word[76];
                    break;
                case "1":
                    EnemyDev.Text = Word[77];
                    break;
                case "2":
                    EnemyDev.Text = Word[78];
                    break;
                case "3":
                    EnemyDev.Text = Word[79];
                    break;
            }
            switch (Convert.ToString(TimeBattle))
            {
                case "0":
                    BattleTime.Text = Word[80];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time1.png");
                    break;
                case "1":
                    BattleTime.Text = Word[81];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time2.png");
                    break;
                case "2":
                    BattleTime.Text = Word[82];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time3.png");
                    break;
                case "3":
                    BattleTime.Text = Word[83];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time4.png");
                    break;
                case "4":
                    BattleTime.Text = Word[84];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time5.png");
                    break;
                case "5":
                    BattleTime.Text = Word[85];
                    PanelPic.BackgroundImage = Image.FromFile("Image/Time6.png");
                    break;
            }
            switch (Convert.ToString(PlaceBattle))
            {
                case "0":
                    BattlePlace.Text = Word[0] + Word[86];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place1.png");
                    break;
                case "1":
                    BattlePlace.Text = Word[0] + Word[87];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place2.png");
                    break;
                case "2":
                    BattlePlace.Text = Word[0] + Word[88];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place3.png");
                    break;
                case "3":
                    BattlePlace.Text = Word[0] + Word[89];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place4.png");
                    break;
                case "4":
                    BattlePlace.Text = Word[0] + Word[90];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place5.png");
                    break;
                case "5":
                    BattlePlace.Text = Word[0] + Word[91];
                    PanelBack.BackgroundImage = Image.FromFile("Image/Place6.png");
                    break;
            }
            GenerateBattleText();
            BattleWeapon.Text = Word[6] + ": " + rWeapon;
            BattleWood.Text = Word[7] + ": " + rWood;
            IDD = rnd.Next(0, 1);
            ButtStat.Visible = false;
            PanelBattle.Visible = true;
            EventButt1.Visible = false;
            EventButt2.Visible = false;
            EventButt3.Visible = false;
            EventButt4.Visible = false;
            EventButt5.Visible = false;
            EventButt6.Visible = false;
            Tactic1.Visible = true;
            Tactic2.Visible = true;
            Tactic3.Visible = true;
            Tactic4.Visible = true;
            Tactic5.Visible = true;
            Tactic6.Visible = true;
            Tactic7.Visible = true;
            Tactic8.Visible = true;
            Tactic9.Visible = true;
            Tactic10.Visible = true;
            SaveGame.Visible = false;
            ButtBattleWon.Visible = false;
            if (EnemyNum >= 6)
            {
                BattleNumEnemy = rnd.Next(2, 7);
            }
            else
            {
                BattleNumEnemy = rnd.Next(1, EnemyNum + 1);
            }
            LabelHowMany2.Text = Word[20] + BattleNumEnemy + Word[19];
            BattleNumOur = 1;
            TrackBattleHowMany.Value = BattleNumOur;
            LabelHowMany.Text = Word[17] + BattleNumOur + Word[19];
            if (rPeople >= 6)
            {
                TrackBattleHowMany.Maximum = 6;
            }
            else
            {
                TrackBattleHowMany.Maximum = rPeople;
            }
        }

        private void GenerateBattleText()
        {
            IDD = rnd.Next(0, 8);
            switch (Convert.ToString(IDD))
            {
                case "0":
                    BattleText.Text += Word[104];
                    break;
                case "1":
                    BattleText.Text += Word[105];
                    break;
                case "2":
                    BattleText.Text += Word[106];
                    break;
                case "3":
                    BattleText.Text += Word[107];
                    break;
                case "4":
                    BattleText.Text += Word[108];
                    break;
                case "5":
                    BattleText.Text += Word[109];
                    break;
                case "6":
                    BattleText.Text += Word[110];
                    break;
                case "7":
                    BattleText.Text += Word[111];
                    break;
            }
        }
        private void Fight()
        {
            BattleText.Text = "";
            int Dmg1 = 0; int Dmg2 = 0; //1 - урон игрока, 2 - урон бота
            int Ini1 = 0; //Инициатива. Если больше нуля - игрок бьёт первый
            {
                //0 - поле, 1 - лес, 2 - деревня, 3 - мост, 4 - холм, 5 - болото
                //НАШИ: 0 - голые руки раш, 1 - вооружёные руки раш, 2 - Засада с оружием, 3 - обстрел позиций, 4 - баррикады, 5 - тяжеловооружёная атака, 6 - организованный раш, 7 - рассыпание
                //ОНИ: 0 - тупой раш, 1 - попытка обороны, 2 - засада, 3 - обстрел позиций, 4 - аккуратная атака, 5 - окоп, 6 - умный раш, 7 - черепаха
                Dmg2 = EnemyDevLVL - DevLVL; // IDT = тактика игрока, IDD = тактика врага?
                if (HaveStat[1] == true) { Ini1++; }
                if (HaveStat[5] == true && PlaceBattle == 2 && BattleType == true) { Dmg2--; }
                Dmg1++; Dmg2++; //Попытка сбалансить тот факт, что никто не умирает
                int D1 = 0; int D2 = 0; int In = 0; //Используется для запоминания бафов от местности и времени суток
                if (TimeBattle == 0) { In -= 1; }
                if (TimeBattle == 1) { D1 += 1; }
                if (TimeBattle == 2) { In += 1; }
                if (TimeBattle == 3) { In -= 1; }
                if (TimeBattle == 4) { In -= 2; }
                if (TimeBattle == 5) { In += 1; D2 += 1; }
                if (PlaceBattle == 0) { D1 -= 1; }
                if (PlaceBattle == 1) { In += 1; }
                if (PlaceBattle == 2) { D2 -= 1; In += 1; }
                if (PlaceBattle == 3) { D1 -= 1; D2 -= 1; }
                if (PlaceBattle == 4) { D1 += 1; In += 1; }
                if (PlaceBattle == 5) { In += 2; }
                if (BattleType == true)
                {
                    Dmg1 += D1; Dmg2 += D2; Ini1 += In;
                }
                else
                {
                    Dmg1 += D2; Dmg2 += D1; Ini1 -= In;
                }
                if (IDT == 0 && IDD == 0) { Dmg1 += 1; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 0 && IDD == 1) { Dmg1 -= 1; Dmg2 += 1; Ini1 -= 1; }
                if (IDT == 0 && IDD == 2) { Dmg1 -= 1; Dmg2 += 1; Ini1 -= 3; }
                if (IDT == 0 && IDD == 3) { Dmg1 -= 1; Dmg2 += 0; Ini1 += 1; }
                if (IDT == 0 && IDD == 4) { Dmg1 += 0; Dmg2 += 2; Ini1 += 0; }
                if (IDT == 0 && IDD == 5) { Dmg1 -= 1; Dmg2 += 2; Ini1 -= 1; }
                if (IDT == 0 && IDD == 6) { Dmg1 += 1; Dmg2 += 0; Ini1 += 1; }
                if (IDT == 0 && IDD == 7) { Dmg1 += 1; Dmg2 -= 1; Ini1 += 2; }
                if (IDT == 1 && IDD == 0) { Dmg1 += 2; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 1 && IDD == 1) { Dmg1 += 1; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 1 && IDD == 2) { Dmg1 += 0; Dmg2 += 1; Ini1 -= 3; }
                if (IDT == 1 && IDD == 3) { Dmg1 -= 1; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 1 && IDD == 4) { Dmg1 += 1; Dmg2 += 2; Ini1 += 0; }
                if (IDT == 1 && IDD == 5) { Dmg1 += 0; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 1 && IDD == 6) { Dmg1 += 1; Dmg2 += 0; Ini1 += 2; }
                if (IDT == 1 && IDD == 7) { Dmg1 += 2; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 2 && IDD == 0) { Dmg1 += 1; Dmg2 += 0; Ini1 += 2; }
                if (IDT == 2 && (IDD == 1 || IDD == 2 || IDD == 5)) { rWeapon++; Dmg1 -= 20; Dmg2 -= 20; Ini1 = 0; }
                if (IDT == 2 && IDD == 3) { Dmg1 -= 3; Dmg2 += 0; Ini1 -= 3; }
                if (IDT == 2 && IDD == 4) { Dmg1 -= 1; Dmg2 += 1; Ini1 -= 1; }
                if (IDT == 2 && IDD == 6) { Dmg1 += 1; Dmg2 -= 1; Ini1 += 1; }
                if (IDT == 2 && IDD == 7) { Dmg1 -= 2; Dmg2 += 0; Ini1 += 2; }
                if (IDT == 3 && IDD == 0) { Dmg1 += 1; Dmg2 += 1; Ini1 -= 1; }
                if (IDT == 3 && IDD == 1) { Dmg1 += 1; Dmg2 -= 20; Ini1 += 2; }
                if (IDT == 3 && IDD == 2) { Dmg1 += 0; Dmg2 -= 20; Ini1 += 3; }
                if (IDT == 3 && IDD == 3) { Dmg1 += 0; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 3 && IDD == 4) { Dmg1 -= 3; Dmg2 -= 1; Ini1 += 3; }
                if (IDT == 3 && IDD == 5) { Dmg1 += 1; Dmg2 -= 20; Ini1 += 3; }
                if (IDT == 3 && IDD == 6) { Dmg1 -= 1; Dmg2 += 0; Ini1 -= 1; }
                if (IDT == 3 && IDD == 7) { Dmg1 -= 3; Dmg2 += 1; Ini1 += 3; }
                if (IDT == 4 && IDD == 0) { Dmg1 += 0; Dmg2 -= 2; Ini1 += 0; }
                if (IDT == 4 && (IDD == 1 || IDD == 2 || IDD == 5)) { Dmg1 -= 20; Dmg2 -= 20; Ini1 += 0; rWood++; }
                if (IDT == 4 && IDD == 3) { Dmg1 += 0; Dmg2 -= 3; Ini1 -= 2; }
                if (IDT == 4 && IDD == 4) { Dmg1 += 0; Dmg2 += 1; Ini1 += 2; }
                if (IDT == 4 && IDD == 6) { Dmg1 -= 1; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 4 && IDD == 7) { Dmg1 -= 2; Dmg2 -= 2; Ini1 += 0; }
                if (IDT == 5 && IDD == 0) { Dmg1 += 2; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 5 && IDD == 1) { Dmg1 += 2; Dmg2 += 1; Ini1 -= 1; }
                if (IDT == 5 && IDD == 2) { Dmg1 += 1; Dmg2 += 0; Ini1 -= 3; }
                if (IDT == 5 && IDD == 3) { Dmg1 += 0; Dmg2 += 1; Ini1 -= 3; }
                if (IDT == 5 && IDD == 4) { Dmg1 += 2; Dmg2 += 1; Ini1 += 1; }
                if (IDT == 5 && IDD == 5) { Dmg1 += 2; Dmg2 += 1; Ini1 -= 2; }
                if (IDT == 5 && IDD == 6) { Dmg1 += 1; Dmg2 += 0; Ini1 += 1; }
                if (IDT == 5 && IDD == 7) { Dmg1 += 1; Dmg2 -= 1; Ini1 += 2; }
                if (IDT == 6 && IDD == 0) { Dmg1 += 0; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 6 && IDD == 1) { Dmg1 += 0; Dmg2 += 1; Ini1 += 1; }
                if (IDT == 6 && IDD == 2) { Dmg1 += 2; Dmg2 -= 2; Ini1 += 0; }
                if (IDT == 6 && IDD == 3) { Dmg1 += 0; Dmg2 += 1; Ini1 -= 5; }
                if (IDT == 6 && IDD == 4) { Dmg1 += 2; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 6 && IDD == 5) { Dmg1 += 2; Dmg2 -= 1; Ini1 -= 1; }
                if (IDT == 6 && IDD == 6) { Dmg1 += 0; Dmg2 += 2; Ini1 -= 1; }
                if (IDT == 6 && IDD == 7) { Dmg1 -= 1; Dmg2 += 0; Ini1 += 2; }
                if (IDT == 7 && IDD == 0) { Dmg1 -= 1; Dmg2 += 1; Ini1 -= 1; }
                if (IDT == 7 && IDD == 1) { Dmg1 -= 2; Dmg2 += 0; Ini1 += 0; }
                if (IDT == 7 && IDD == 2) { Dmg1 += 0; Dmg2 -= 2; Ini1 += 0; }
                if (IDT == 7 && IDD == 3) { Dmg1 -= 1; Dmg2 -= 3; Ini1 += 0; }
                if (IDT == 7 && IDD == 4) { Dmg1 += 0; Dmg2 += 1; Ini1 -= 2; }
                if (IDT == 7 && IDD == 5) { Dmg1 -= 1; Dmg2 += 1; Ini1 += 0; }
                if (IDT == 7 && IDD == 6) { Dmg1 -= 1; Dmg2 -= 1; Ini1 += 0; }
                if (IDT == 7 && IDD == 7) { Dmg1 -= 3; Dmg2 += 0; Ini1 += 0; }
            } //Просто огромная скобка чтоб прятать условия
            Ini1 += BattleNumOur / 2; //За каждого бойца +0.5 к инициативе
            Ini1 -= BattleNumEnemy / 2; //За каждого вражеского -0.5 к инициативе
            Dmg1 += BattleNumOur / 2;
            Dmg2 += BattleNumEnemy / 2; //Аналогично с уроном
            System.Console.WriteLine("Бонус урона от числа наш " + (BattleNumOur / 2) + ". Бонус от числа врага " + (BattleNumEnemy / 2));
            if (Ini1 > 0) //Инициатива выше нуля, значит игрок бьёт первым (при нуле - всё же враг)
            {
                if (Ini1 >= 5) //Ошеломляющая
                {
                    BattleText.Text = Word[115] + Ini1 + ")\n";
                    Dmg1 += 1;
                }
                else //Обычная (иного варианта нет)
                {
                    BattleText.Text = Word[114] + Ini1 + ")\n";
                }
                if (Dmg1 < 0) { Dmg1 = 0; } //Проверки на то, чтоб дамаг в минус не ушёл
                if (Dmg2 < 0) { Dmg2 = 0; }
                if (Dmg2 > BattleNumOur && Ini1 > -5) { Dmg2 = BattleNumOur; } //Если не ошеломляющая атака, но враг нанёс много урона - ограничить
                if (Dmg1 > BattleNumEnemy && Ini1 < 5) { Dmg1 = BattleNumEnemy; } //Если не ошеломляющая атака, но мы нанесли много урона - ограничить
                EnemyNum -= Dmg1;
                if (EnemyNum > 0) //Если враг остался жив - контратакует
                {
                    rPeople -= Dmg2; //Контратака врага
                }
                BattleText.Text += Word[121] + Dmg1 + Word[122] + "\n";
                BattleText.Text += Word[119] + Dmg2 + Word[120] + "\n";
            } // Проверка инициативы
            else //Инициатива нулевая или ниже. Враг бьёт первым
            {
                if (Ini1 == 0) //Ничья
                {
                    BattleText.Text = Word[116] + Ini1 + ")\n";
                    Dmg1 -= 1;
                    Dmg2 -= 1;
                }
                if (Ini1 > -5 && Ini1 < 0) //Обычная 
                {
                    BattleText.Text = Word[113] + Ini1 + ")\n";
                }
                if (Ini1 <= -5) //Ошеломляющая
                {
                    BattleText.Text = Word[112] + Ini1 + ")\n";
                    Dmg2 += 1;
                }
                if (Dmg1 < 0) { Dmg1 = 0; } //Проверки на то, чтоб дамаг в минус не ушёл
                if (Dmg2 < 0) { Dmg2 = 0; }
                if (Dmg2 > BattleNumOur && Ini1 > -5) { Dmg2 = BattleNumOur; } //Если не ошеломляющая атака, но враг нанёс много урона - ограничить
                if (Dmg1 > BattleNumEnemy && Ini1 < 5) { Dmg1 = BattleNumEnemy; } //Если не ошеломляющая атака, но мы нанесли много урона - ограничить
                rPeople -= Dmg2;
                if (rPeople > 0) //Если враг остался жив - контратакует
                {
                    EnemyNum -= Dmg1; //Ваша контратака 
                }
                BattleText.Text += Word[121] + Dmg1 + Word[122] + "\n";
                BattleText.Text += Word[119] + Dmg2 + Word[120] + "\n";
            }
            LaberYouMan.Text = Word[1] + ": " + rPeople;
            LaberEnemyMan.Text = Word[2] + ": " + EnemyNum;
            BattleWeapon.Text = Word[99] + rWeapon;
            BattleWood.Text = Word[100] + rWood;
            GenerateBattleText();
            if (rPeople <= 0)
            {
                PanelBattle.Visible = false;
                GG();
            }
            else
            {
                BattleNumOur = 1;
                TrackBattleHowMany.Value = BattleNumOur;
                LabelHowMany.Text = Word[17] + BattleNumOur + Word[19];
                if (rPeople >= 6)
                {
                    TrackBattleHowMany.Maximum = 6;
                }
                else
                {
                    TrackBattleHowMany.Maximum = rPeople;
                }
            }
            if (EnemyNum <= 0)
            {
                Tactic1.Visible = false;
                Tactic2.Visible = false;
                Tactic3.Visible = false;
                Tactic4.Visible = false;
                Tactic5.Visible = false;
                Tactic6.Visible = false;
                Tactic7.Visible = false;
                Tactic8.Visible = false;
                Tactic9.Visible = false;
                Tactic10.Visible = false;
                BattleRetrate.Visible = false;
                ButtBattleWon.Visible = true;
                ButtBattleWon.Text = Word[10];
                BattleText.Text = Word[52];
                int i = 0;
                if (EnemyDevLVL == 0)
                    i = rnd.Next(0, 3);
                if (EnemyDevLVL == 1)
                    i = rnd.Next(1, 4);
                if (EnemyDevLVL == 2)
                    i = rnd.Next(2, 6);
                rWeapon += i;
                BonWeapon.Text = "+" + i;
                BonusShow.Start();
            }
            else
            {
                if (EnemyNum >= 6)
                {
                    BattleNumEnemy = rnd.Next(2, 7);
                }
                else
                {
                    BattleNumEnemy = rnd.Next(1, EnemyNum + 1);
                }
                LabelHowMany2.Text = Word[20] + BattleNumEnemy + Word[19];
            }
            System.Console.WriteLine("Инициатива " + Ini1 + ". Урон ваш " + Dmg1 + ". Урон врага " + Dmg2);
        }

        private void BonusShow_Tick(object sender, EventArgs e)
        {
            ShutDownString();
        }

        private void ShutDownString()
        {
            BonusShow.Stop();
            BonFood.Text = "";
            BonMorale.Text = "";
            BonGold.Text = "";
            BonWood.Text = "";
            BonPeople.Text = "";
            BonWeapon.Text = "";
            BonMetal.Text = "";
            BonHeal.Text = "";
            BonStatus.Text = "";
        }

        private void TradeTrack_Scroll(object sender, EventArgs e)
        {
            EventButt1.Text = "";
            switch (Convert.ToString(T)) //1 - люди, 2 - еда, 3 хилки, 4 - обыч. ресы, 5 - металл, 6 - оружие, 7 - алмазы. T - продаёт, B - требует
            {
                case "1":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[53];
                    break;
                case "2":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[54];
                    break;
                case "3":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[55];
                    break;
                case "4":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[56];
                    break;
                case "5":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[57];
                    break;
                case "6":
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[58];
                    break;
                case "7":
                    R = 1;
                    EventButt1.Text += "" + TradeTrack.Value * R + Word[59];
                    break;
            }
            switch (Convert.ToString(B)) //1 - люди, 2 - еда, 3 хилки, 4 - обыч. ресы, 5 - металл, 6 - оружие, 7 - алмазы. T - продаёт, B - требует
            {
                case "1":
                    P = 1;
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[60];
                    break;
                case "2":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[61];
                    break;
                case "3":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[62];
                    break;
                case "4":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[63];
                    break;
                case "5":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[64];
                    break;
                case "6":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[65];
                    break;
                case "7":
                    EventButt1.Text += "" + (TradeTrack.Value * P) + Word[66];
                    break;
            }
        }

        private void ButtStat_Click(object sender, EventArgs e)
        {
            LaberRepTribe.Text = Word[67] + RepTribe + Word[70];
            LaberRepCas.Text = Word[68] + RepCastle + Word[70];
            LaberRepCity.Text = Word[69] + RepCity + Word[70];
            switch (Convert.ToString(DevLVL))
            {
                case "0":
                    LaberDev.Text = Word[48];
                    break;
                case "1":
                    LaberDev.Text = Word[49];
                    break;
                case "2":
                    LaberDev.Text = Word[50];
                    break;
                case "3":
                    LaberDev.Text = Word[51];
                    break;
            }
            for (int i = 0; i < HaveAch.Length; i++)
            {
                if (HaveAch[i] == true)
                {
                    list_labels[i].ForeColor = Color.Green;
                    list_labels[i].Text = AchText[i];
                }
                else
                {
                    list_labels[i].ForeColor = Color.Red;
                    list_labels[i].Text = Word[71];
                }
            }
            for (int i = 0; i < 12; i++)
            {
                System.Console.WriteLine("Загрузили ачивку номер " + i);
                if (HaveStat[i] == true)
                {
                    list_labels2[i].Text = StatText[i];
                }
                else
                {
                    list_labels2[i].Text = "";
                }
            }
            PanelStatus.Visible = true;
            ButtStat.Visible = false;
            PanelStatus.Location = new Point(10, 10);
        }

        private void CloseStatus_Click(object sender, EventArgs e)
        {
            PanelStatus.Visible = false;
            ButtStat.Visible = true;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[0] == true)
            {
                StatusDesc.Text = AchDesc[0];
            }
        }
        private void label2_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[1] == true)
            {
                StatusDesc.Text = AchDesc[1];
            }
        }
        private void label3_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[2] == true)
            {
                StatusDesc.Text = AchDesc[2];
            }
        }
        private void label4_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[3] == true)
            {
                StatusDesc.Text = AchDesc[3];
            }
        }
        private void label5_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[4] == true)
            {
                StatusDesc.Text = AchDesc[4];
            }
        }
        private void label6_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[5] == true)
            {
                StatusDesc.Text = AchDesc[5];
            }
        }
        private void label7_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[6] == true)
            {
                StatusDesc.Text = AchDesc[6];
            }
        }
        private void label8_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[7] == true)
            {
                StatusDesc.Text = AchDesc[7];
            }
        }
        private void label9_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[8] == true)
            {
                StatusDesc.Text = AchDesc[8];
            }
        }
        private void label10_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[9] == true)
            {
                StatusDesc.Text = AchDesc[9];
            }
        }
        private void label11_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[10] == true)
            {
                StatusDesc.Text = AchDesc[10];
            }
        }
        private void label12_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[11] == true)
            {
                StatusDesc.Text = AchDesc[11];
            }
        }
        private void label13_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[12] == true)
            {
                StatusDesc.Text = AchDesc[12];
            }
        }
        private void label14_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[13] == true)
            {
                StatusDesc.Text = AchDesc[13];
            }
        }
        private void label15_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[14] == true)
            {
                StatusDesc.Text = AchDesc[14];
            }
        }
        private void label16_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[15] == true)
            {
                StatusDesc.Text = AchDesc[15];
            }
        }
        private void label17_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[16] == true)
            {
                StatusDesc.Text = AchDesc[16];
            }
        }
        private void label18_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[17] == true)
            {
                StatusDesc.Text = AchDesc[17];
            }
        }
        private void label19_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[18] == true)
            {
                StatusDesc.Text = AchDesc[18];
            }
        }
        private void label20_MouseEnter(object sender, EventArgs e)
        {
            if (HaveAch[19] == true)
            {
                StatusDesc.Text = AchDesc[19];
            }
        }

        private void PanelStatus_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = "";
        }

        private void TitleStatus_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[47];
        }

        private void TitleStatus2_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[46];
        }

        private void LaberRepTribe_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[45] + "\n" + Word[41] + "\n" + Word[42] + "\n" + Word[43];
        }

        private void LaberRepCas_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[44] + "\n" + Word[41] + "\n" + Word[42] + "\n" + Word[43];
        }

        private void LaberRepCity_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[40] + "\n" + Word[41] + "\n" + Word[42] + "\n" + Word[43];
        }

        private void LaberDev_MouseEnter(object sender, EventArgs e)
        {
            StatusDesc.Text = Word[39];
        }

        private void PanelBattle_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[21];
        }

        private void Tactic1_Click(object sender, EventArgs e)
        {
            IDT = 0;
            Fight();
        }
        private void Tactic2_Click(object sender, EventArgs e)
        {
            if (rWeapon >= 1)
            {
                if (rnd.Next(1,3) == 2)
                    rWeapon -= 1;
                IDT = 1;
                Fight();
            }
        }
        private void Tactic3_Click(object sender, EventArgs e)
        {
            if (rWeapon >= 1)
            {
                if (rnd.Next(1, 3) == 2)
                    rWeapon -= 1;
                IDT = 2;
                Fight();
            }
        }
        private void Tactic4_Click(object sender, EventArgs e)
        {
            if (rWeapon >= 1)
            {
                if (rnd.Next(1, 3) == 2)
                    rWeapon -= 1;
                IDT = 3;
                Fight();
            }
        }
        private void Tactic5_Click(object sender, EventArgs e)
        {
            if (rWood >= 1)
            {
                if (rnd.Next(1, 3) == 2)
                    rWood -= 1;
                IDT = 4;
                Fight();
            }
        }
        private void Tactic6_Click(object sender, EventArgs e)
        {
            if (rWeapon >= 2)
            {
                if (rnd.Next(1, 3) == 2)
                    rWeapon -= 1;
                if (rnd.Next(1, 3) == 2)
                    rWeapon -= 1;
                IDT = 5;
                Fight();
            }
        }
        private void Tactic7_Click(object sender, EventArgs e)
        {
            if (rWeapon >= 1)
            {
                if (rnd.Next(1, 3) == 2)
                    rWeapon -= 1;
                IDT = 6;
                Fight();
            }
        }
        private void Tactic8_Click(object sender, EventArgs e)
        {
            IDT = 7;
            Fight();
        }
        private void Tactic9_Click(object sender, EventArgs e)
        {

        }
        private void Tactic10_Click(object sender, EventArgs e)
        {

        }

        private void Tactic1_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[28];
        }
        private void Tactic2_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[29] +  "\n" + Word[30];
        }
        private void Tactic3_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[31] + "\n" + Word[30];
        }
        private void Tactic4_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[32] + "\n" + Word[30];
        }
        private void Tactic5_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[33] + "\n" + Word[34];
        }
        private void Tactic6_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[35] + "\n" + Word[36];
        }
        private void Tactic7_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[37] + "\n" + Word[30];
        }
        private void Tactic8_MouseEnter(object sender, EventArgs e)
        {
            BattleTacText.Text = Word[38];
        }
        private void Tactic9_MouseEnter(object sender, EventArgs e)
        {

        }
        private void Tactic10_MouseEnter(object sender, EventArgs e)
        {

        }

        private void ButtBattleWon_Click(object sender, EventArgs e)
        {
            RemembaStat[0]++;
            if (RemembaStat[0] == 5)
            {
                HaveAch[5] = true;
            }
            SaveGame.Visible = true;
            BattleGo = false;
            PanelBattle.Visible = false;
            ButtStat.Visible = true;
            PanelBack.Dispose();
            EventAlgoritm();
            EventLoad();
        }

        private void Status1_MouseEnter(object sender, EventArgs e)
        {
            int i = 0;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status2_MouseEnter(object sender, EventArgs e)
        {
            int i = 1;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status3_MouseEnter(object sender, EventArgs e)
        {
            int i = 2;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status4_MouseEnter(object sender, EventArgs e)
        {
            int i = 3;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status5_MouseEnter(object sender, EventArgs e)
        {
            int i = 4;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status6_MouseEnter(object sender, EventArgs e)
        {
            int i = 5;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status7_MouseEnter(object sender, EventArgs e)
        {
            int i = 6;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status8_MouseEnter(object sender, EventArgs e)
        {
            int i = 7;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status9_MouseEnter(object sender, EventArgs e)
        {
            int i = 8;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status10_MouseEnter(object sender, EventArgs e)
        {
            int i = 9;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status11_MouseEnter(object sender, EventArgs e)
        {
            int i = 10;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Status12_MouseEnter(object sender, EventArgs e)
        {
            int i = 11;
            StatusDesc.Text = Word[i + StatText.Length + NumOfWord] + "\n" + Word[i + StatText.Length*2 + NumOfWord];
        }

        private void Music_Tick(object sender, EventArgs e)
        {
            
            if (MusicON == false)
            {
                Music.Stop();
            }
            else
            {
                int i = rnd.Next(1, 4); // Всегда делать рандом на 1 меньше, чем максимальный. Т.е. если rnd.Next(1, 9), то первый i = 1 последний i = 8
                string Path = System.AppDomain.CurrentDomain.BaseDirectory;

                if (i == 1)
                {
                    Music.Interval = 210000;
                }
                if (i == 2)
                {
                    Music.Interval = 258000;
                }
                if (i == 3)
                {
                    Music.Interval = 162000;
                }
                PlayFile(Path + @"\Music\Tribe" + i + ".mp3");
            }
        }


        private void PlayFile(String url)
        {
            Player = new WMPLib.WindowsMediaPlayer();
            Player.URL = url;
            Player.controls.play();
            Player.settings.volume = Volume.Value;
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            Player.settings.volume = Volume.Value;
        }

        private void BattleRetrate_Click(object sender, EventArgs e)
        {
            PanelBack.Dispose();
            rMorale -= 25;
            BonMorale.Text = "-25";
            BonusShow.Start();
            SaveGame.Visible = true;
            BattleGo = false;
            PanelBattle.Visible = false;
            ButtStat.Visible = true;
            EventAlgoritm();
            EventLoad();
        }

        private void LangLeft_Click(object sender, EventArgs e)
        {
            Lang--;
            LangCheck();
        }

        private void LangRight_Click(object sender, EventArgs e)
        {
            Lang++;
            LangCheck();
        }

        private void LangCheck()
        {
            if (Lang < 0)
                Lang = 2;
            if (Lang > 2)
                Lang = 0;
            if (Lang == 0)
            {
                Text = File.ReadAllText("rus.txt");
                LangTextBox.Text = "rus.txt";
            }
            if (Lang == 1)
            {
                Text = File.ReadAllText("eng.txt");
                LangTextBox.Text = "eng.txt";
            }
            if (Lang == 2)
            {
                Text = File.ReadAllText("bel.txt");
                LangTextBox.Text = "bel.txt";
            }
            LoadText();
            LangText.Text = Word[123];
            StartGame.Text = Word[14];
            LoadGame.Text = Word[15];
            VolumeText.Text = Word[13];
            this.Text = Word[124];
            StatusDesc.Text = Word[127];
            TitleStatus.Text = Word[125];
            TitleStatus2.Text = Word[126];
            ButtStat.Text = Word[126];
            SaveGame.Text = Word[16];
            BattleAllOnIt.Text = Word[18];
            for (int i = 0; i < StatText.Length; i++ )
            {
                StatText[i] = Word[i + NumOfWord];
            }
            for (int i = 0; i < AchDesc.Length; i++)
            {
                AchText[i] = Word[i + StatText.Length*3 + NumOfWord];
                AchDesc[i] = Word[i + StatText.Length*3 + AchText.Length + NumOfWord];
            }
        }

        private void TrackBattleHowMany_Scroll(object sender, EventArgs e)
        {
            BattleNumOur = TrackBattleHowMany.Value;
            LabelHowMany.Text = Word[17] + BattleNumOur + Word[19];
        }

        private void BattleAllOnIt_Click(object sender, EventArgs e)
        {
            BattleNumOur = TrackBattleHowMany.Maximum;
            TrackBattleHowMany.Value = BattleNumOur;
            LabelHowMany.Text = Word[17] + BattleNumOur + Word[19];
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Oem3:
                    if (ConOpen != true)
                    {
                        Butt[0] = new Button
                        {
                            Width = 100,
                            Height = 20,
                            Location = new Point(120, 0),
                            Name = "ConsolButt",
                            Text = " Enter"
                        };
                        Consol = new TextBox
                        {
                            Width = 120,
                            Location = new Point(0, 0),
                            Name = "Consol",
                        };
                        this.Butt[0].Click += new EventHandler(ConsolWork);
                        EventPicture.Controls.Add(Butt[0]);
                        EventPicture.Controls.Add(Consol);
                        Butt[0].Visible = true;
                        Consol.Visible = true;
                        ConOpen = true;
                        //Consol.Visible = false;
                        //ConsolEnter.Visible = false;
                    }
                    else
                    {
                        ConOpen = false;
                        Butt[0].Dispose();
                        Consol.Dispose();
                        //Consol.Visible = true;
                        //ConsolEnter.Visible = true;
                    }
                    break;
                //case Keys.Escape:
                //    if (Menu.Visible == true)
                //    {
                //        Menu.Visible = false;                                тут, типо, в будущем мб норм меню будет, как в беллумке, со звуком и выходом, агась...
                //    }
                //    else
                //    {
                //        Menu.Location = new Point(306, 120);
                //        Menu.Visible = true;
                //        if (MusicON == true)
                //        {
                //            ButtMusicTurn.Text = "Музыка: Выключить";
                //            TrackVol.Visible = true;
                //            TextMusic.Visible = true;
                //        }
                //        else
                //        {
                //            ButtMusicTurn.Text = "Музыка: Включить";
                //            TrackVol.Visible = false;
                //            TextMusic.Visible = false;
                //        }
                //    }
                //    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        } //Сворована из беллумки  
        protected void ConsolWork(object sender, EventArgs e)
        {
        if (sender is Button button)
        {
                if (Consol.Text == "Лох")
                {
                    MessageBox.Show("Сам такой");
                }
                if ((Consol.Text).Contains("Battle"))
                {
                    EnemyDevLVL = Convert.ToByte((Consol.Text).Substring(6));
                    TimeBattle = Convert.ToByte(rnd.Next(0, 6));
                    PlaceBattle = rnd.Next(0, 6);
                    EnemyNum = rnd.Next(3, 8);
                    BattleType = false;
                    Battle();
                    if (BattleGo == false)
                    {
                        EventAlgoritm();
                        EventLoad();
                    }
                    BonusSee();
                    BonusShow.Start();
                }
                Consol.Text = "";
        }
    } //Сворована из челябы

        protected void RestartGame(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //Тут рестартовые штуки?
                Day = 0;
                BattleGo = false;

                NumDay.Visible = true;
                SaveGame.Visible = true;
                NumFood.Visible = true;
                NumGold.Visible = true;
                NumHeal.Visible = true;
                NumMetal.Visible = true;
                NumMorale.Visible = true;
                NumPeople.Visible = true;
                NumWeapon.Visible = true;
                NumWoodStone.Visible = true;
                BonHeal.Visible = true;
                BonGold.Visible = true;
                BonFood.Visible = true;
                BonMetal.Visible = true;
                BonMorale.Visible = true;
                BonPeople.Visible = true;
                BonWeapon.Visible = true;
                BonWood.Visible = true;
                ButtStat.Visible = true;
                Starting();
                Butt[1].Dispose();
            }
        } 
    }
}
