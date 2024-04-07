using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System;

namespace L3_ItemCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Item : INotifyPropertyChanged
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _quality = "";
        public string Quality
        {
            get { return _quality; }
            set
            {    
                _quality = value;
                OnPropertyChanged(nameof(Quality)); 
            }
        }

        private int _level = 0;
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        private string _iType = "";
        public string IType
        {
            get { return _iType; }
            set
            {
                _iType = value;
                OnPropertyChanged(nameof(IType));
            }
        }

        public bool UniqueEquipped { get; set; }
        public string Mesh { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Item(string name, string quality, string iType, int level, bool uniqueEquipped, string mesh)
        {
            Name = name;
            Quality = quality;
            IType = iType;
            Level = level;
            UniqueEquipped = uniqueEquipped;
            Mesh = mesh;
        }
    }

    public class FileManager
    {
        public string FilePath = "";
        private readonly JsonSerializerOptions JsonSO = new() { WriteIndented = true };

        public ObservableCollection<Item> LoadItems()
        {
            ObservableCollection<Item> items = [];
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    items = JsonSerializer.Deserialize<ObservableCollection<Item>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading items: {ex.Message}");
            }
            return items;
        }

        public void SaveItems(ObservableCollection<Item> items)
        {
            try
            {

                string json = JsonSerializer.Serialize(items, JsonSO);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving items: {ex.Message}");
            }
        }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _mFileName = "No File";
        public string MFileName
        {
            get { return _mFileName; }
            set
            {
                _mFileName = value;
                OnPropertyChanged(nameof(MFileName));
            }
        }

        private string _mFilePath = "No File";
        public string MFilePath {
            get { return _mFilePath; }
            set
            {
                _mFilePath = value;
                MFileName = System.IO.Path.GetFileName(value);
                OnPropertyChanged(nameof(MFilePath));
            }
        }

        private ObservableCollection<Item> _itemDB = [];
        public ObservableCollection<Item> ItemDB
        {
            get { return _itemDB; }
            set
            {
                _itemDB = value;
                OnPropertyChanged(nameof(ItemDB));
            }
        }

        public FileManager FM = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            /*
            ItemDB =
            [
                new Item ( "Shadowmourne", "Legendary", "2H Axe", 10, true, "Pyramid" ),
                new Item ( "Undaunting Breastplate", "Rare", "Chest", 340, false, "Cube" ),
                new Item ( "Ashkandur", "Epic", "2H Sword", 456, true, "Pyramid" ),
                new Item ( "Tiny Basket", "Uncommon", "Off-hand", 23, true, "Pyramid" ),
                new Item ( "Apparatus of Khaz'goroth", "Epic", "Trinket", 256, false, "Sphere" ),
                new Item ( "Common Leather Boots", "Common", "Boots", 674, false, "Cube" ),
                new Item ( "Pebble", "Poor", "Junk", 89, true, "Sphere" ),
            ];//*/
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var item in ItemDB)
            //{
            //    Debug.WriteLine(item.Mesh);
            //}

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TypeTextBox.Text) ||
                ItemQuality.SelectedItem == null ||
                string.IsNullOrWhiteSpace(LevelTextBox.Text) ||
                !MeshRadioButtonContainer.Children.OfType<RadioButton>().Any(rb => rb.IsChecked == true))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = NameTextBox.Text;
            string type = TypeTextBox.Text;
            string? quality = (ItemQuality.SelectedItem as ComboBoxItem)?.Content.ToString();
            int.TryParse(LevelTextBox.Text, out int level);
            string? mesh = MeshRadioButtonContainer.Children.OfType<RadioButton>()
                            .FirstOrDefault(r => r.IsChecked == true)?.Content.ToString();
            bool uniqueEquipped = UniqueEquippedCheckBox.IsChecked == true;

            Item newItem = new(name, quality, type, level, uniqueEquipped, mesh);
            ItemDB.Add(newItem);
            ItemDB = new ObservableCollection<Item>(ItemDB.OrderBy(item => item.Name));

            ClearInputFields();
        }

        private void ClearInputFields()
        {
            NameTextBox.Text = "";
            TypeTextBox.Text = "";
            ItemQuality.SelectedIndex = 1;
            LevelTextBox.Text = "";
            ((RadioButton)MeshRadioButtonContainer.Children[1]).IsChecked = true;
            UniqueEquippedCheckBox.IsChecked = false;
        }

        private void Discard_Button_Click(object sender, RoutedEventArgs e)
        {
            ItemDB[0].Name = "Fyralath";
            ItemDB[0].Quality = "Legendary";
            ItemDB[0].Level = 496;
            ItemDB[0].IType = "2H Axe";
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Do you want to create a new file?", "New File", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    MFilePath = CreateNewFile(filePath);
                    //MessageBox.Show($"New file created: {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            //}
        }

        public static string CreateNewFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Create the new file at the specified path
                using (FileStream fs = File.Create(filePath))
                {
                    // byte[] initialContent = Encoding.UTF8.GetBytes("Initial content");
                    // fs.Write(initialContent, 0, initialContent.Length);
                }
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "Error";
            }
        }

        public void OpenExistingFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Open File",
                    Filter = "All Files (*.*)|*.*",
                    InitialDirectory = Directory.GetCurrentDirectory()
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    MFilePath = openFileDialog.FileName;
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}