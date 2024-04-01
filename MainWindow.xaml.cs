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
using static System.Net.Mime.MediaTypeNames;

namespace L3_ItemCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class Item
    {
        public string Name { get; set; }
        public string Quality { get; set; }
        public string IType { get; set; }
        public int Level { get; set; }
        public bool UniqueEquipped { get; set; }
        public string Mesh { get; set; }

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


    public partial class MainWindow : Window
    {
    List<Item> ItemDB;
        public MainWindow()
        {
            InitializeComponent();
            
            ItemDB = new List<Item>
            {
                new Item ( "Shadowmourne", "Legendary", "2H Axe", 10, true, "Pyramid" ),
                new Item ( "Undaunting Breastplate", "Rare", "Chest", 340, false, "Cube" ),
                new Item ( "Ashkandur", "Epic", "2H Sword", 456, true, "Pyramid" ),
                new Item ( "Tiny Basket", "Uncommon", "Off-hand", 23, true, "Pyramid" ),
                new Item ( "Apparatus of Khaz'goroth", "Epic", "Trinket", 256, false, "Sphere" ),
                new Item ( "Common Leather Boots", "Common", "Boots", 674, false, "Cube" ),
                new Item ( "Pebble", "Poor", "Junk", 89, true, "Sphere" ),
            };

            //myListBox.Items.Add("Item 1");

            DataContext = this;

            

            itemDBListBox.ItemsSource = ItemDB;
        }

        private void ItemQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}