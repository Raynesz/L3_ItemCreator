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

namespace L3_ItemCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Item(string name, string quality, string iType, int level, bool uniqueEquipped, string mesh)
    {
        public string Name { get; set; } = name;
        public string Quality { get; set; } = quality;
        public string IType { get; set; } = iType;
        public int Level { get; set; } = level;
        public bool UniqueEquipped { get; set; } = uniqueEquipped;
        public string Mesh { get; set; } = mesh;
    }

    public class FileManager
    {
        public string FilePath = "";
        private readonly JsonSerializerOptions JsonSO = new() { WriteIndented = true };

        public List<Item> LoadItems()
        {
            List<Item> items = [];
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    items = JsonSerializer.Deserialize<List<Item>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading items: {ex.Message}");
            }
            return items;
        }

        public void SaveItems(List<Item> items)
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
        private string _mFilePath = "No File";
        public string MFilePath {
            get { return _mFilePath; }
            set
            {
                _mFilePath = value;
                OnPropertyChanged(nameof(MFilePath));
            }
        }
        private List<Item> _itemDB = [];
        public List<Item> ItemDB
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

            //MFilePath = "No File";
            ItemDB =
            [
                new Item ( "Shadowmourne", "Legendary", "2H Axe", 10, true, "Pyramid" ),
                new Item ( "Undaunting Breastplate", "Rare", "Chest", 340, false, "Cube" ),
                new Item ( "Ashkandur", "Epic", "2H Sword", 456, true, "Pyramid" ),
                new Item ( "Tiny Basket", "Uncommon", "Off-hand", 23, true, "Pyramid" ),
                new Item ( "Apparatus of Khaz'goroth", "Epic", "Trinket", 256, false, "Sphere" ),
                new Item ( "Common Leather Boots", "Common", "Boots", 674, false, "Cube" ),
                new Item ( "Pebble", "Poor", "Junk", 89, true, "Sphere" ),
            ];
            
            //myListBox.Items.Add("Item");
            //itemDBListBox.ItemsSource = ItemDB;
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            MFilePath = "123";
        }

        private void Discard_Button_Click(object sender, RoutedEventArgs e)
        {
            List<Item> TempDB = ItemDB;
            TempDB[2].Quality = "Uncommon";
            ItemDB = TempDB;
            Debug.WriteLine(ItemDB[2].Name);
            Debug.WriteLine(ItemDB[2].Quality);
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