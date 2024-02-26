using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace File_Manager
{
    [Serializable]
    public class MyListBox
    {
        public string Login = "admin111";
        public string Password = "123456";
        public List<string> PathsWithRightsAdmin = new List<string>();
        public int[] ColorForRGB = new int[3];
        public Font MyFont = new Font("Arial", 14);
        public int MySize = 14;
        [OnSerializing]
        internal void OnSerializedMethod(StreamingContext context)
        {
            //MessageBox.Show(Login);
            string alhpabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string new_login = "";
            string new_password = "";
            for (int i = 0; i < Login.Length; i++)
            {
                if (alhpabet.Contains(Login[i]))
                {
                    int k = alhpabet.IndexOf(Login[i]);
                    new_login += alhpabet[k + 1];
                }
                else { new_login += Login[i]; }
            }
            for (int i = 0; i < Password.Length; i++)
            {
                if (alhpabet.Contains(Password[i]))
                {
                    int k = alhpabet.IndexOf(Password[i]);
                    //new_password += alhpabet[k + 1];
                }
                else { new_password += Password[i]; }
            }
            Password = "lll";
            Login = new_login;
            //MessageBox.Show(Login);
        }
        [OnDeserialized]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            //MessageBox.Show(Login);
            string alhpabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string new_login = "";
            string new_password = "";
            for (int i = 0; i < Login.Length; i++)
            {
                if (alhpabet.Contains(Login[i]))
                {
                    int k = alhpabet.IndexOf(Login[i]);
                    new_login += alhpabet[k - 1];
                }
                else { new_login += Login[i]; }
            }
            for (int i = 0; i < Password.Length; i++)
            {
                if (alhpabet.Contains(Password[i]))
                {
                    int k = alhpabet.IndexOf(Password[i]);
                    new_password += alhpabet[k - 1];
                }
                else { new_password += Password[i]; }
            }
            Password = new_password;
            Login = new_login;
            //MessageBox.Show(Login);
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;
        }
        //settings 
        BinaryFormatter formatter = new BinaryFormatter();
        //задаем форму
        private void Change_Size()
        {
            //WindowState = FormWindowState.Maximized;                                        
            Size = new Size(650, 650);
            CenterToScreen();
            MaximizeBox = false; ///запрещаем менять размеры формы
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "File Manager";
        }
        //создаем выпадающий список
        ComboBox btn = new ComboBox();
        string output;
        private void CreateButton()
        {
            btn.Width = 100;
            btn.Height = 75;
            btn.BackColor = Color.FromArgb(211, 211, 211);
            btn.Items.Add("C:\\");
            btn.Items.Add("D:\\");
            btn.SelectedIndexChanged += btn_SelectedIndexChanged;
            Controls.Add(btn);
        }
        void btn_SelectedIndexChanged(object sender, EventArgs e)
        {
            output = btn.SelectedItem.ToString();
            Trace_Label.Text = output;
            FileListBox.Items.Clear();
            DisplayFiles(output);
        }
        //создаем label с текущим путем до файла
        Label Trace_Label = new Label();
        private void CreateLabel()
        {
            Trace_Label.Location = new Point(0,btn.Height);
            Trace_Label.Width = 300;
            Trace_Label.Height = 25;
            Trace_Label.BackColor = Color.FromArgb(211, 211, 211);
            Controls.Add(Trace_Label);
        }
        //создаем листобокс с файлами в данной папке
        ListBox FileListBox = new ListBox();
        MyListBox FakeListBox = new MyListBox();
        private void SerializationListBox()
        {
            using (FileStream fs = new FileStream(@"C:\Users\VLADIMIR\Desktop\mylistbox.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, FakeListBox);
                //MessageBox.Show(FakeListBox.Login);
                fs.Close();
            }
        }
        private void DeSerializationListBox()
        {
            // десериализация из файла @"C:\Users\Пользователь\Desktop\mylistbox.dat"
            using (FileStream fs = new FileStream(@"C:\Users\VLADIMIR\Desktop\mylistbox.dat", FileMode.Open))
            {
                FakeListBox = (MyListBox)formatter.Deserialize(fs);
                FileListBox.ForeColor = Color.FromArgb(FakeListBox.ColorForRGB[0], FakeListBox.ColorForRGB[1], FakeListBox.ColorForRGB[2]);
                FileListBox.Font = FakeListBox.MyFont;
                FileListBox.Font = new Font(FileListBox.Name, FakeListBox.MySize);
                fs.Close();
            }
            //MessageBox.Show(FakeListBox.Login);
        }

        private void CreateListBox()
        {
            //DeSerializationListBox();
            FileListBox.Location = new Point(0, Trace_Label.Height + btn.Height);
            FileListBox.Height = 600;
            FileListBox.Width = 600;
            FileListBox.SelectedIndexChanged += FileListBox_DoubleClick;
            FileListBox.SelectionMode = SelectionMode.MultiExtended;
            Controls.Add(FileListBox);
        }
        //вывод файлов
        string currentLocation = "";
        //panel_FilesList.Controls.Clear();
        bool displayfiles;
        private void DisplayFiles(string filePath)
        {
            string[] filesList = Directory.GetDirectories(filePath).Concat(Directory.GetFiles(filePath)).ToArray();
            currentLocation = filePath;
            if (filesList.Length != 0)
            {
                FileListBox.Items.Clear();
                FileListBox.Items.Add("Назад");
                for (int i = 0; i < filesList.Length - 1; i++)
                {
                    bool isHidden = ((File.GetAttributes(filesList[i]) & FileAttributes.Hidden) == FileAttributes.Hidden);
                    if (!isHidden)
                    {
                        // Get the name of the file from the path
                        var startOfName = filesList[i].LastIndexOf("\\");
                        var fileName = filesList[i].Substring(startOfName + 1, filesList[i].Length - (startOfName + 1));
                        // Display the file or folder as a button
                        FileListBox.Items.Add(fileName);
                    }
                }
            }
            else { displayfiles = false; }
            displayfiles = true;
        }
        //текущий путь до файла/папки
        string CurrentOoldPath = "";
        string t = "";

        char[] chars_split = new[] { ' ', '.', ',', ';', ':', '?', '!', '\n', '\r', '\t', '-', ':' };
        private void FileListBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                t = FileListBox.SelectedItem.ToString();
            }
            catch { }
            if (t == "Назад")
            {
                //Directory.SetCurrentDirectory(output);
                string previousFolder = output.Substring(0, output.LastIndexOf("\\"));
                if (previousFolder.Contains("\\"))
                {
                    output = previousFolder.Substring(0, previousFolder.LastIndexOf("\\")) + "\\";
                }
                else
                {
                    switch (previousFolder)
                    {
                        case "C:":
                            output = "D:\\";
                            break;
                        case "D:":
                            output = "C:\\";
                            break;
                        default:
                            break;
                    }
                }
                Trace_Label.Text = output;
                DisplayFiles(output);
            }
            else if (t.Length > 0)
            {
                output += t + "\\";
                CurrentOoldPath = output;
                string ant = output.Substring(0, output.LastIndexOf("\\"));
                if (CheckRights(ant))
                {
                    if (File.Exists(ant))
                    {
                        output = output.Substring(0, output.LastIndexOf("\\"));
                        output = output.Substring(0, output.LastIndexOf("\\")) + "\\";
                    }
                    else
                    {
                        try
                        {
                            //Directory.SetCurrentDirectory(output);
                            string filePath = Path.GetFullPath(output);
                            Trace_Label.Text = output;
                            try
                            {
                                // If a directory clicked, reload list of files in new directory
                                DisplayFiles(filePath);
                                if (displayfiles == false)
                                {
                                    output = output.Substring(0, output.LastIndexOf("\\"));
                                    output = output.Substring(0, output.LastIndexOf("\\")) + "\\";
                                    Trace_Label.Text = output;
                                }
                            }
                            catch (Exception ex)
                            {
                                // If file clicked, open the file
                                //var process = new System.Diagnostics.Process();
                                //process.StartInfo = new System.Diagnostics.ProcessStartInfo() { UseShellExecute = true, FileName = filePath };
                                //process.Start();
                            }
                        }
                        catch
                        {
                            output = output.Substring(0, output.LastIndexOf("\\"));
                            output = output.Substring(0, output.LastIndexOf("\\")) + "\\";
                            Trace_Label.Text = output;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы не обладаете правами администратора");
                }
             }
        }
        
        
        
        //выпадающее меню для настроек
        MenuStrip mn = new MenuStrip();       
       
        private void CreateSettings()
        {
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            ToolStripMenuItem colortext = new ToolStripMenuItem("Color");
            ToolStripMenuItem fonttext = new ToolStripMenuItem("Font");
            ToolStripMenuItem sizetext = new ToolStripMenuItem("Size");
            ToolStripMenuItem addrights = new ToolStripMenuItem("Add Rights");
            ToolStripMenuItem deleterights = new ToolStripMenuItem("Delete Rights");
            fonttext.Click += FontText;
            colortext.Click += ColorText;
            sizetext.Click += SizeText;
            addrights.Click += AddRights;
            deleterights.Click += DeleteRights;
            settingsItem.DropDownItems.Add(colortext);
            settingsItem.DropDownItems.Add(fonttext);
            settingsItem.DropDownItems.Add(sizetext);
            settingsItem.DropDownItems.Add(addrights);
            settingsItem.DropDownItems.Add(deleterights);
            mn.Items.Add(settingsItem);
        }
        //проверить права админа по пути
        private bool CheckRights(string t)
        {
            if (!(FakeListBox.PathsWithRightsAdmin.Contains(t)))
            {
                return true;
            }
            string cur_login = Microsoft.VisualBasic.Interaction.InputBox("Введите логин");
            string cur_password = Microsoft.VisualBasic.Interaction.InputBox("Введите пароль");
            if (cur_login == FakeListBox.Login && cur_password == FakeListBox.Password)
            {
                return true;
            }
            return false;

        }
        //добавить права админа файлу/папке
        private void AddRights(object sender, EventArgs e)
        {
            string cur_login = Microsoft.VisualBasic.Interaction.InputBox("Введите логин");
            string cur_password = Microsoft.VisualBasic.Interaction.InputBox("Введите пароль");
            string thispath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            if (cur_login == FakeListBox.Login && cur_password == FakeListBox.Password)
            {
                FakeListBox.PathsWithRightsAdmin.Add(thispath);
                //SerializationListBox();
            }
        }
        //удалить права админа файлу/папке
        private void DeleteRights(object sender, EventArgs e)
        {
            string cur_login = Microsoft.VisualBasic.Interaction.InputBox("Введите логин");
            string cur_password = Microsoft.VisualBasic.Interaction.InputBox("Введите пароль");
            string thispath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            if (cur_login == FakeListBox.Login && cur_password == FakeListBox.Password)
            {
                if (FakeListBox.PathsWithRightsAdmin.Contains(thispath))
                {
                    FakeListBox.PathsWithRightsAdmin.Remove(thispath);
                    //SerializationListBox();
                }
            }
        }
        //поменять размера текста
        private void SizeText(object sender, EventArgs e)
        {
            string newpath = Microsoft.VisualBasic.Interaction.InputBox("Введите размер шрифта\nПример: 13 ");
            int currentsize = Convert.ToInt32(newpath.Replace(" ", ""));
            FileListBox.Font = new Font(FileListBox.Font.Name, currentsize);
            FakeListBox.MySize = currentsize;
            //SerializationListBox();
        }
        //поменять шрифт текста
        private void FontText(object sender, EventArgs e)
        {
            string newpath = Microsoft.VisualBasic.Interaction.InputBox("Введите название шрифта\nПример: Arial ");
            newpath = newpath.Replace(" ", "");
            FileListBox.Font = new Font(newpath, FakeListBox.MySize);
            FakeListBox.MyFont = new Font(newpath, FakeListBox.MySize);
            //SerializationListBox();
        }
        //поменять цвет текста
        private void ColorText(object sender, EventArgs e)
        {
            string newpath = Microsoft.VisualBasic.Interaction.InputBox("Введите цвет в РГБ через запятую \n Пример: 0,255,255");
            newpath = newpath.Replace(" ", "");
            string[] array_newpath = newpath.Split(',');
            int color1 = Convert.ToInt32(array_newpath[0]) ;
            int color2 = Convert.ToInt32(array_newpath[1]);
            int color3 = Convert.ToInt32(array_newpath[2]);
            FileListBox.ForeColor = Color.FromArgb(color1,color2,color3);
            FakeListBox.ColorForRGB[0] = color1;
            FakeListBox.ColorForRGB[1] = color2;
            FakeListBox.ColorForRGB[2] = color3;
            //SerializationListBox();
        }
      
        //операций с файлами/каталогами
        private void CreateFunction()
        {
            mn.Dock = DockStyle.Right;
            ToolStripMenuItem count_lines = new ToolStripMenuItem("Count_Lines");
            ToolStripMenuItem count_words = new ToolStripMenuItem("Count_Words");
            ToolStripMenuItem popular_words = new ToolStripMenuItem("TOP10");


            ToolStripMenuItem fileItem = new ToolStripMenuItem("File");
            ToolStripMenuItem deletefile = new ToolStripMenuItem("Delete");
            ToolStripMenuItem renamefile = new ToolStripMenuItem("Rename");
            ToolStripMenuItem copyfile = new ToolStripMenuItem("Copy");
            ToolStripMenuItem movefile = new ToolStripMenuItem("Move");
            ToolStripMenuItem archivefile = new ToolStripMenuItem("Archive");
            ToolStripMenuItem nonarchivefile = new ToolStripMenuItem("UnArchive");

            popular_words.Click += Popular_Words_Click;
            count_lines.Click += Count_lines_Click;
            count_words.Click += Count_words_Click;


            renamefile.Click += RenameFile;
            deletefile.Click += DeleteFile;
            copyfile.Click += CopyFile;
            movefile.Click += ClickFile;
            archivefile.Click += ArchiveFile;
            nonarchivefile.Click += NotArchiveFile;

            fileItem.DropDownItems.Add(count_lines);
            fileItem.DropDownItems.Add(count_words);
            fileItem.DropDownItems.Add(popular_words);


            fileItem.DropDownItems.Add(renamefile);
            fileItem.DropDownItems.Add(deletefile);
            fileItem.DropDownItems.Add(copyfile);
            fileItem.DropDownItems.Add(movefile);
            fileItem.DropDownItems.Add(archivefile);
            fileItem.DropDownItems.Add(nonarchivefile);
            mn.Items.Add(fileItem);
            CreateSettings();
            Controls.Add(mn);
        }
        //want to show top 10  popular words (length > 5) in txt
        

        private void Popular_Words_Click(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            if (File.Exists(oldpath))
            {
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();

                string lines = File.ReadAllText(oldpath);
                string[] words = lines.Split(chars_split, StringSplitOptions.RemoveEmptyEntries);

                var wordCount1 = words.AsParallel()
                    .Where(word => word.Length > 5)
                    .GroupBy(word => word, StringComparer.OrdinalIgnoreCase)         
                    .Select(group => new { Word = group.Key, Count = group.Count() })
                    .OrderByDescending(item => item.Count)
                    .Take(10)
                    .ToList();



                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();

                var wordCount2 = words
                    .Where(word => word.Length > 5)
                    .GroupBy(word => word, StringComparer.OrdinalIgnoreCase)
                    .Select(group => new { Word = group.Key, Count = group.Count() })
                    .OrderByDescending(item => item.Count)
                    .Take(10)
                    .ToList(); 
                stopwatch2.Stop();
                string output_this = "";
                foreach (var v in wordCount1)
                {
                    output_this += "Слово \"" + v.Word + $"\" встретилось {v.Count} раз" + "\n";
                }
                MessageBox.Show(output_this + Environment.NewLine + "Time with AsParallel: " + stopwatch1.ElapsedMilliseconds + " мс" +
                    Environment.NewLine + "Time without AsParallel: " + stopwatch2.ElapsedMilliseconds + " мс");

            }

        }

        //want to calculate words in txt (mean that punctuation marks are words) 
        private void Count_words_Click(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            if (File.Exists(oldpath))
            {
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();

                string[] lines = File.ReadAllLines(oldpath);

                // Use LINQ to split lines into words and count them
                int wordCount = lines.AsParallel()
                    .SelectMany(line => line.Split(chars_split , StringSplitOptions.RemoveEmptyEntries))
                    .Count();



                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();

                int wordCount2 = lines
                    .SelectMany(line => line.Split(chars_split, StringSplitOptions.RemoveEmptyEntries))
                    .Count(); 
                stopwatch2.Stop();
                
                MessageBox.Show($"В данном тексте - {wordCount2},{wordCount} слов" + Environment.NewLine + "Time with AsParallel: " + stopwatch1.ElapsedMilliseconds + " мс" +
                    Environment.NewLine + "Time without AsParallel: " + stopwatch2.ElapsedMilliseconds + " мс");
                
            }
        }


        //want to calculate lines in txt 
        private void Count_lines_Click(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\")); 
            if (File.Exists(oldpath))
            {
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();


                //var wordlook = new HashSet<string>(
                //    File.ReadAllLines(oldpath).AsParallel()).ToList();
                var count_lines = (from n in File.ReadAllLines(oldpath).AsParallel() select n).Count();
                stopwatch1.Stop();

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();

                var count_lines_2 = (from n in File.ReadAllLines(oldpath) select n).Count();
                stopwatch2.Stop();

                MessageBox.Show($"В данном тексте - {count_lines},{count_lines_2} строк" + Environment.NewLine + "Time with AsParallel: " + stopwatch1.ElapsedMilliseconds + " мс" +
                    Environment.NewLine + "Time without AsParallel: " + stopwatch2.ElapsedMilliseconds + " мс");
            }
        }

        //метод разархивировать zip 
        private void NotArchiveFile(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            string newpath = oldpath.Substring(0, oldpath.LastIndexOf("."));
            ZipFile.ExtractToDirectory(oldpath, newpath);

        }
        //метод архивировать файл/папку
        private void ArchiveFile(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\")); 
            if (File.Exists(oldpath))
            {
                string newpath = oldpath.Substring(0, oldpath.LastIndexOf("."));
                using (var fileStream = new FileStream(newpath + ".zip", FileMode.Create))
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(oldpath, oldpath.Substring(oldpath.LastIndexOf("\\")+1));
                }
            }
            else
            {
                string newpath = oldpath.Substring(0, oldpath.LastIndexOf("\\"));
                newpath += oldpath.Substring(oldpath.LastIndexOf("\\")) + ".zip";
                ZipFile.CreateFromDirectory(oldpath, newpath);
            }
        }
        //метод переместить данную папку/файл в  другой 
        private void ClickFile(object sender, EventArgs e)
        {
            string newpath = Microsoft.VisualBasic.Interaction.InputBox("Введите новый путь для файла/папки");
            newpath += "\\"+t;
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\")); ;
            if (File.Exists(oldpath))
            {
                File.Move(oldpath, newpath);
            }
            else
            {
                CopyDir(oldpath, newpath);
                DeleteDir(oldpath);
                output = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\";
                Trace_Label.Text = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\";
                DisplayFiles(oldpath.Substring(0, oldpath.LastIndexOf("\\")));
            }
        }   
        //метод скопировать данную папку/файл в  другой 
        private void CopyFile(object sender, EventArgs e)
        {
            string newpath = Microsoft.VisualBasic.Interaction.InputBox("Введите путь до файла/папки копирования");
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\")); 
            if (File.Exists(oldpath))
            {
                File.Copy(oldpath,newpath);
                DisplayFiles(oldpath.Substring(0, oldpath.LastIndexOf("\\")));
            }
            else
            {
                CopyDir(oldpath, newpath);
            }
        }
        private void CopyDir(string FromDir, string ToDir)
        {
            string DirName = FromDir.Substring(FromDir.LastIndexOf("\\"));
            ToDir += "\\" + DirName;
            Directory.CreateDirectory(ToDir);
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                string s2 = ToDir + "\\" + Path.GetFileName(s1);
                File.Copy(s1, s2);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                CopyDir(s, ToDir + "\\" + Path.GetFileName(s));
            }
        }
        //метод удалить данную папку/файл
        private void DeleteFile(object sender, EventArgs e)
        {
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\")); ;
            if (File.Exists(oldpath))
            {
                File.Delete(oldpath);
                DisplayFiles(oldpath.Substring(0, oldpath.LastIndexOf("\\")));
            }
            else
            {
                DeleteDir(oldpath);
                output = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\";
                Trace_Label.Text = oldpath.Substring(0, oldpath.LastIndexOf("\\"))+"\\";
                DisplayFiles(oldpath.Substring(0, oldpath.LastIndexOf("\\")));
            }
        }
        private void DeleteDir(string FromDir)
        {
            foreach (string s1 in Directory.GetFiles(FromDir))
            {
                File.Delete(s1);
            }
            foreach (string s in Directory.GetDirectories(FromDir))
            {
                DeleteDir(s);
            }
            try { Directory.Delete(FromDir); }
            catch { }
        }
        //метод переименовать данную папку/файл
        private void RenameFile(object sender, EventArgs e)
        {
            string NewName = Microsoft.VisualBasic.Interaction.InputBox("Введите новое имя для файла/папки:");
            string oldpath = CurrentOoldPath.Substring(0, CurrentOoldPath.LastIndexOf("\\"));
            string newpath = oldpath.Substring(0, output.LastIndexOf("\\")) + "\\" + NewName;
            if (File.Exists(oldpath) && newpath.Length>0)
            {
                File.Move(oldpath, newpath);
                DisplayFiles(newpath.Substring(0, newpath.LastIndexOf("\\")));
            }
            else
            {
                string newpath_dyrectory = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\" + NewName;
                Directory.Move(oldpath, newpath_dyrectory);
                output = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\";
                Trace_Label.Text = oldpath.Substring(0, oldpath.LastIndexOf("\\")) + "\\";
                DisplayFiles(oldpath.Substring(0, oldpath.LastIndexOf("\\")));
            }
        }
        //при закрытии формы делаю сериализацию
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SerializationListBox();
            //MessageBox.Show(FakeListBox.Login);
        }
        //форма
        private void Form1_Load(object sender, EventArgs e)
        {
            //при открытии формы делаю десериализацию
            try
            {
                DeSerializationListBox();
            }
            catch { }
            Change_Size();
            CreateButton();
            CreateLabel();
            CreateListBox();
            CreateFunction();
            //Trace_Label.Text = FakeListBox.Login;
        }
    }
}
