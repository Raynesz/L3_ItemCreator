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

    public static class FileManager
    {
        private readonly static JsonSerializerOptions JsonSO = new() { WriteIndented = true };

        public static string NewFile ()
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
                return CreateNewFile(filePath);
                //MessageBox.Show($"New file created: {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                return "";
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

        public static string OpenFile()
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
                    return openFileDialog.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "";
            }
        }

        public static ObservableCollection<Item> LoadItems(string filePath)
        {
            ObservableCollection<Item> items = [];
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    items = JsonSerializer.Deserialize<ObservableCollection<Item>>(json) ?? [];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading items: {ex.Message}");
            }
            return items;
        }

        public static void SaveItems(ObservableCollection<Item> items, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(items, JsonSO);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving items: {ex.Message}");
            }
        }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _mFilePath = "";
        public string MFilePath {
            get { return _mFilePath; }
            set
            {
                _mFilePath = value;
                OnPropertyChanged(nameof(MFilePath));
                OnPropertyChanged(nameof(MFileName));
            }
        }

        private string GetFileName()
        {
            string fileName = System.IO.Path.GetFileName(MFilePath);
            if (fileName == "") return "No File";
            else return fileName;
        }

        public string MFileName => GetFileName();

        private string GetSelectedItemName()
        {
            Item SelectedItem = (Item)ItemDBListBox.SelectedItem;
            if (SelectedItem == null) return "New Item";
            return SelectedItem.Name; 
        }

        public string SelectedItemName => GetSelectedItemName();

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

        public readonly static RoutedCommand NewCommand = new();
        public readonly static RoutedCommand OpenCommand = new();
        public readonly static RoutedCommand SaveCommand = new();
        public readonly static RoutedCommand NewItemCommand = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //*
            ItemDB =
            [
                new Item ( "Shadowmourne", "Legendary", "2H Axe", 10, true, "Pyramid" ),
                new Item ( "Undaunting Breastplate", "Rare", "Chest", 340, false, "Cube" ),
                new Item ( "Ashkandur", "Epic", "2H Sword", 456, true, "Pyramid" ),
                new Item ( "Tiny Basket", "Uncommon", "Off-hand", 23, true, "Pyramid" ),
                new Item ( "Apparatus of Khaz'goroth", "Epic", "Trinket", 256, false, "Sphere" ),
                new Item ( "Common Leather Boots", "Common", "Boots", 674, false, "Cube" ),
                new Item ( "Pebble", "Poor", "Junk", 89, true, "Sphere" ),
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
            if (InputsAreInvalid())
            {
                MessageBox.Show("Please fill in all required fields with a proper value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = NameTextBox.Text;
            string type = TypeTextBox.Text;
            string quality = (ItemQuality.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Common";
            if (!int.TryParse(LevelTextBox.Text, out int level)) level = 0;
            string mesh = MeshRadioButtonContainer.Children
                              .OfType<RadioButton>()
                              .FirstOrDefault(r => r.IsChecked == true)?
                              .Content?
                              .ToString() ?? "Sphere";
            bool uniqueEquipped = UniqueEquippedCheckBox.IsChecked == true;

            Item newItem = new(name, quality, type, level, uniqueEquipped, mesh);
            ItemDB.Add(newItem);

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

        private bool InputsAreInvalid() {
            return string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                NameTextBox.Text == "New Item" ||
                NameTextBox.Text == "No File" ||
                string.IsNullOrWhiteSpace(TypeTextBox.Text) ||
                ItemQuality.SelectedItem == null ||
                string.IsNullOrWhiteSpace(LevelTextBox.Text) ||
                !MeshRadioButtonContainer.Children.OfType<RadioButton>().Any(rb => rb.IsChecked == true);
        }

        private void Discard_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputsAreInvalid())
            {
                MessageBox.Show("Please fill in all required fields with a proper value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ItemDBListBox.SelectedItem != null)
            {               
                string name = NameTextBox.Text;
                string type = TypeTextBox.Text;
                string quality = (ItemQuality.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Common";
                if (!int.TryParse(LevelTextBox.Text, out int level)) level = 0;
                string mesh = MeshRadioButtonContainer.Children
                              .OfType<RadioButton>()
                              .FirstOrDefault(r => r.IsChecked == true)?
                              .Content?
                              .ToString() ?? "Sphere";
                bool uniqueEquipped = UniqueEquippedCheckBox.IsChecked == true;

                Item selectedItem = (Item)ItemDBListBox.SelectedItem;
                int selectedIndex = ItemDB.IndexOf(selectedItem);
                ItemDB[selectedIndex] = new(name, quality, type, level, uniqueEquipped, mesh);
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ItemDBListBox.SelectedItem != null)
            {
                Item selectedItem = (Item)ItemDBListBox.SelectedItem;
                ItemDB.Remove(selectedItem);
            }
        }

        private void HandleMenuItem_New(object sender, RoutedEventArgs e)
        {
            MFilePath = FileManager.NewFile();
        }

        private void HandleMenuItem_NewItem(object sender, RoutedEventArgs e)
        {
            ItemDBListBox.SelectedItem = null;
            ClearInputFields();
        }

        public void HandleMenuItem_Open(object sender, RoutedEventArgs e)
        {
            MFilePath = FileManager.OpenFile();
        }

        private void HandleMenuItem_Save(object sender, RoutedEventArgs e)
        {
            //FileManager.SaveItems(ItemDB, MFilePath);
            Debug.WriteLine("save");
        }

        private void HandleMenuItem_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ItemDBListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedItemName));
            if (ItemDBListBox.SelectedItem != null)
            {
                Item selectedItem = (Item)ItemDBListBox.SelectedItem;
                ItemDBListBox.SelectedItem = selectedItem;

                NameTextBox.Text = selectedItem.Name;
                TypeTextBox.Text = selectedItem.IType;
                ItemQuality.SelectedIndex = GetItemQualityIndex(selectedItem.Quality);
                LevelTextBox.Text = selectedItem.Level.ToString();
                ((RadioButton)MeshRadioButtonContainer.Children[GetMeshIndex(selectedItem.Mesh)]).IsChecked = true;
                UniqueEquippedCheckBox.IsChecked = selectedItem.UniqueEquipped;

                createButton.Visibility = Visibility.Collapsed;
                discardButton.Visibility = Visibility.Collapsed;
                saveButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                createButton.Visibility = Visibility.Visible;
                discardButton.Visibility = Visibility.Visible;
                saveButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;

                ClearInputFields();
            }
        }

        private static int GetItemQualityIndex(string quality)
        {
            return quality switch
            {
                "Poor" => 0,
                "Common" => 1,
                "Uncommon" => 2,
                "Rare" => 3,
                "Epic" => 4,
                "Legendary" => 5,
                _ => 1,
            };
        }

        private static int GetMeshIndex(string mesh)
        {
            return mesh switch
            {
                "Cube" => 0,
                "Sphere" => 1,
                "Pyramid" => 2,
                _ => 1,
            };
        }
    }
}