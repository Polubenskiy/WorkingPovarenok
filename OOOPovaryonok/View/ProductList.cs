using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OOOPovaryonok.Classes;
using OOOPovaryonok.Entities;

namespace OOOPovaryonok.View
{
    public partial class ProductList : Form
    {
        public ProductList() => InitializeComponent();

        public ProductList(Roles role)
        {
            InitializeComponent();
            this.role = role;

            MessageBox.Show($"id роли = {role.roleID} имя роли = {role.nameRole}");
        }

        Bitmap bitmap;
        string pathExe = Application.StartupPath;       //Путь к exe без имени файла
        int filterDiscount, filterCategory;         //Фильтр по скидке и мануфактуре
        string sort;                        //Направление сортировки
        int countAll, countFilter;          //Количество всех записей и после фильтрации
        string search;                  //Строка поиска
                                        //Массив скидок
        string[,] arrayDiscount = new string[,] { { "0", "100" }, { "0", "9.99" }, { "10", "14.99" }, { "15", "100" } };

        Roles role = new Roles();


        private void ProductList_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ProductList_Load(object sender, System.EventArgs e)
        {
            dataGridViewProductList.Rows.Clear();
            this.dataGridViewProductList.Columns[1].Resizable = DataGridViewTriState.False;


            // Clear Categoria
            coloredComboCategoria.Items.Clear();
            coloredComboCategoria.Items.Add("Все категории");

            coloredComboSort.Items.Clear();
            coloredComboSort.Items.Add("По возврастанию");
            coloredComboSort.Items.Add("По убыванию");
            coloredComboSort.Text = coloredComboSort.Items[0].ToString();

            coloredComboDiscount.Items.Clear();
            coloredComboDiscount.Items.Add("Все скидки");
            coloredComboDiscount.Items.Add("0-9 %");
            coloredComboDiscount.Items.Add("10-14 %");
            coloredComboDiscount.Items.Add("15 % и более");
            coloredComboDiscount.Text = coloredComboDiscount.Items[0].ToString();

            // Insert data from DataBase
            string sqlCategory = "SELECT * FROM [Category]";
            SqlCommand command = new SqlCommand(sqlCategory, SQLHelper.connection);
            SqlDataReader sqlDataReader = command.ExecuteReader();

            if (sqlDataReader.HasRows)
                while (sqlDataReader.Read())
                {
                    coloredComboCategoria.Items.Add(sqlDataReader.GetString(sqlDataReader.GetOrdinal("CategoryName")));
                }
            sqlDataReader.Close();
            coloredComboCategoria.Text = coloredComboCategoria.Items[0].ToString();

            filterDiscount = 0;
            filterCategory = 0;
            sort = "ASC";

            DisplayProducts();

        }

        private void DisplayProducts()
        {
            // Общее количество товаров в таблице товаров
            string sqlCount = "SELECT COUNT (*) FROM [Product]";

            using (SqlCommand commandCountProduct = SQLHelper.connection.CreateCommand())
            {
                commandCountProduct.CommandText = sqlCount;
                countAll = (int)commandCountProduct.ExecuteScalar();
            }


            //User user = new User();
            //SQLDatabaseHelper.DatabaseContext.User.Add(user);
            
         
            //Все поля таблицы товаров и производителей
            string sqlProducts = "Select * From [Product], [Manufacture] " +
                "where Product.ProductManufacturerID = Manufacture.ManufactureID";

            //Фильтрация по скидке
            string filterDiscountString = " AND [Product].ProductDiscountMax BETWEEN " +
                                      arrayDiscount[filterDiscount, 0] + " AND " + arrayDiscount[filterDiscount, 1];
            sqlProducts += filterDiscountString;
            //Фильтрация по категории
            if (filterCategory != 0)
            {
                string filterCategoryString = " AND [Product].ProductCategoryID=" + filterCategory;
                sqlProducts += filterCategoryString;
            }
            //Поиск по названию
            string searchString = " AND [Product].ProductName LIKE '%" + search + "%'";
            sqlProducts += searchString;
            //Сортировка
            string sortString = " ORDER BY [Product].ProductCost " + sort;
            sqlProducts += sortString;
                       
            using (SqlCommand sqlCommandFindProduct = SQLHelper.connection.CreateCommand())
            {
                sqlCommandFindProduct.CommandText = sqlProducts;
                countFilter = 0;
                dataGridViewProductList.Rows.Clear();
                SqlDataReader sqlDataReader = sqlCommandFindProduct.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        int index = dataGridViewProductList.Rows.Add();
                        dataGridViewProductList.Rows[index].Cells[0].Value = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductArticleNumber"));

                        string photo = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductPhoto"));

                        if (string.IsNullOrEmpty(photo))
                        {
                            bitmap = Properties.Resources.placeholder; // заглушка для пустого изобразения

                            GetDataFromDatabase(sqlDataReader, index);
                            
                            countFilter++;		//Число записей, попавших под фильтр
                        }
                        else
                        {
                            string pathToDirectory = Application.StartupPath;
                            string pathToImate = pathToDirectory + "\\Товары\\" + photo;

                            if (File.Exists(pathToImate))
                                bitmap = new Bitmap(pathToImate);
                            else
                                bitmap = Properties.Resources.placeholder;  // заглушка

                            GetDataFromDatabase(sqlDataReader, index);
                            
                            countFilter++;		//Число записей, попавших под фильтр
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Нет данных о товарах");
                }

                sqlDataReader.Close();
            }

            labelCountProduct.Text = countFilter + " из " + countAll;
        }

        public void GetDataFromDatabase(SqlDataReader sqlDataReader, int index)
        {
            string description = "Наименование товара: " + sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductName")) + Environment.NewLine;
            description += "Описание товара: " + sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductDescription")) + Environment.NewLine;
            description += "Производитель товара: " + sqlDataReader.GetString(sqlDataReader.GetOrdinal("ManufactureName")) + Environment.NewLine;

            double costProduct = Convert.ToDouble(sqlDataReader.GetDecimal(sqlDataReader.GetOrdinal("ProductCost")));
            description += "Стоимость товара: " + costProduct + Environment.NewLine;

            double discountProduct = Convert.ToDouble(sqlDataReader.GetByte(sqlDataReader.GetOrdinal("ProductDiscountNow")));
            description += "Скидка " + discountProduct + " % " + Environment.NewLine;

            costProduct -= costProduct * discountProduct / 100;
            description += "Стоимость товара со скидкой " + costProduct;

            dataGridViewProductList.Rows[index].Cells[1].Value = bitmap;
            dataGridViewProductList.Rows[index].Cells[2].Value = description; // Описание 
            dataGridViewProductList.Rows[index].Height = 200;

            //Задание цвета для строк при заданных условиях
            if (sqlDataReader.GetByte(sqlDataReader.GetOrdinal("ProductDiscountMax")) > 15)
                dataGridViewProductList.Rows[countFilter].DefaultCellStyle.BackColor = Color.FromArgb(204, 102, 0);
            else
                dataGridViewProductList.Rows[countFilter].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
        }

        // Изменение направления сортировки
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            sort = "ASC";
            if (coloredComboSort.SelectedIndex == 1)
                sort = "DESC";
            DisplayProducts();
        }

        /// Выбор фильтрации по скидке
        private void comboBoxDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            //filterDiscount = comboBoxDiscount.SelectedIndex;    
            DisplayProducts();
        }

        private void dataGridViewProductList_SelectionChanged(object sender, EventArgs e)
        {
            dataGridViewProductList.ClearSelection();
        }

        private void tableLayoutPanelNavigation_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridViewProductList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// Фильтр по категории
        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterCategory = coloredComboCategoria.SelectedIndex;
            DisplayProducts();
        }

        /// Ввод строки для поиска
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            search = textBoxSearch.Text;
            DisplayProducts();
        }

    }
}
