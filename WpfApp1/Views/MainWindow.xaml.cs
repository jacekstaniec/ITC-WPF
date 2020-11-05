using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using WpfApp1.Models;
using System.Data;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //        List<Header> myHeaders = new List<Header>();
        //        List<Detail> myDetails = new List<Detail>();

        DataTable dtHeader = new DataTable();
        DataTable dtDetail = new DataTable();
        string ConString;

        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();
            GitRepo();

            void FillDataGrid()
            {
                // string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                string DbPath = System.IO.Path.GetFullPath("Database1.mdf");
                ConString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = " + DbPath + "; Integrated Security = True";
                string sqlHeader = string.Empty;
                string sqlDetail = string.Empty;

                using (SqlConnection con = new SqlConnection(ConString))
                {
                    sqlHeader = "SELECT Id, Customer_id, Header_name, Date FROM Headers";
                    sqlDetail = "SELECT Id, Header_id, Article_name, Quantity, Net, Gross FROM Details";
                    // TODO try catch
                    SqlCommand cmdHeader = new SqlCommand(sqlHeader, con);
                    SqlCommand cmdDetail = new SqlCommand(sqlDetail, con);
                    SqlDataAdapter sdaHeader = new SqlDataAdapter(cmdHeader);
                    SqlDataAdapter sdaDetail = new SqlDataAdapter(cmdDetail);
                    dtHeader = new DataTable("H");
                    dtDetail = new DataTable("D");
                    // dtHeader.PrimaryKey = new DataColumn[] { dtHeader.Columns["Id"] };
                    sdaHeader.Fill(dtHeader);
                    sdaDetail.Fill(dtDetail);
                    dgHeaders.ItemsSource = dtHeader.DefaultView;
                    // dgDetails.ItemsSource = dtDetail.DefaultView;

                    #region
                    //con.Open();
                    //using (SqlDataReader reader = cmdHeader.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        int _id = Convert.ToInt16(reader["Id"]);
                    //        int _customer_id = Convert.ToInt16(reader["Customer_id"]);
                    //        string _header_name = Convert.ToString(reader["Header_name"]);
                    //        DateTime _date = Convert.ToDateTime(reader["Date"]);
                    //        Header Hed1 = new Header() { Id = _id, Customer_id = 1, Header_name = _header_name, Date = _date };
                    //        myHeaders.Add(Hed1);
                    //    }
                    //    reader.Close();
                    //}
                    //CmdString = string.Empty;
                    //CmdString = "SELECT Id, Header_id, Article_name, Quantity, Net, Gross FROM Details";
                    //SqlCommand cmdDetail = new SqlCommand(CmdString, con);
                    //using (SqlDataReader reader = cmdDetail.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        int _id = Convert.ToInt16(reader[0]);
                    //        int _header_id = Convert.ToInt16(reader[1]);
                    //        string _article_name = Convert.ToString(reader[2]);
                    //        int _quantity = Convert.ToInt16(reader[3]);
                    //        SqlMoney _net = (SqlMoney)Convert.ToSingle(reader[4]);
                    //        SqlMoney _gross = (SqlMoney)Convert.ToSingle(reader[5]);
                    //        Detail Det1 = new Detail() { Id = _id, Header_id = _header_id, Article_name = _article_name, Quantity = _quantity, Net = _net, Gross = _gross };
                    //        myDetails.Add(Det1);
                    //    }
                    //    reader.Close();
                    //}
                    //con.Close();
                    //dgHeaders.ItemsSource = myHeaders;
                    //dgDetails.ItemsSource = myDetails;
                    #endregion
                }
            }
        }


        private void dgh_NewArticle(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);

            try
            {
                var Res = MessageBox.Show("Do you want to Add new article", "Confirm", MessageBoxButton.YesNo);
                if (Res == MessageBoxResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(ConString))
                    {
                        string values = String.Format(" VALUES ( '{0}' , '{1}', '{2}', '{3}', '{4}' )", param.ToString(), "", "0", "0", "0");
                        SqlCommand crud_command = new SqlCommand();
                        crud_command.Connection = con;
                        con.Open();
                        crud_command.CommandText = "INSERT Details (header_id,article_name,quantity,net,gross) " + values;
                        crud_command.ExecuteNonQuery();
                        con.Close();
                    }
                    MessageBox.Show("new article added");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ListCollectionView collectionView = new ListCollectionView((System.Collections.IList)dtDetail.DefaultView);
            //      ListCollectionView collectionView = new ListCollectionView((System.Collections.IList)dgDetails.ItemsSource);
            collectionView.Filter = (ff) =>
            {
                DataRowView d = ff as DataRowView;
                if (d != null)
                {
                    if ((Int32)d.Row.ItemArray[1] == param) return true; return false;    // druga kolumna z tabeli Details => header_id
                }
                return false;
            };
            dgDetails.ItemsSource = collectionView;
        }


        private void dgh_ShowDetails(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);

            ListCollectionView collectionView = new ListCollectionView((System.Collections.IList)dtDetail.DefaultView);
            //      ListCollectionView collectionView = new ListCollectionView((System.Collections.IList)dgDetails.ItemsSource);
            collectionView.Filter = (ff) =>
            {
                DataRowView d = ff as DataRowView;
                if (d != null)
                {
                    if ((Int32)d.Row.ItemArray[1] == param) return true; return false;    // druga kolumna z tabeli Details => header_id
                }
                return false;
            };
            dgDetails.ItemsSource = collectionView;
        }

        private void dgh_DeleteHeader(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);
            try
            {
                var Res = MessageBox.Show("Do you want to Delete this header", "Confirm", MessageBoxButton.YesNo);
                if (Res == MessageBoxResult.Yes)
                {
                    dtHeader.Rows.RemoveAt(_index);
                    using (SqlConnection con = new SqlConnection(ConString))
                    {
                        SqlCommand crud_command = new SqlCommand();
                        crud_command.Connection = con;
                        con.Open();
                        crud_command.CommandText = "DELETE FROM Details WHERE Header_Id = " + param.ToString();
                        crud_command.ExecuteNonQuery();
                        crud_command.CommandText = "DELETE FROM Headers WHERE Id = " + param.ToString();
                        crud_command.ExecuteNonQuery();
                        con.Close();
                    }
                    MessageBox.Show("header and header's details deleted");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgh_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGridRow dgr = e.Row as DataGridRow;
            DataGrid dtr = sender as DataGrid;
            DataRowView drv = dgHeaders.Items[dtr.SelectedIndex] as DataRowView;
            if (!drv.IsNew && drv.IsEdit)
            {
                try
                {
                    var Res = MessageBox.Show("Do you want to Update this new entry", "Confirm", MessageBoxButton.YesNo);
                    if (Res == MessageBoxResult.Yes)
                    {
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        /// <summary>
        /// procedura dodaje lub updatuje dane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgh_AddToDb(object sender, RoutedEventArgs e)
        {
// TODO: rozbić na osobne produry
            DataRowView drv = dgHeaders.SelectedItem as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("data not completed");
                return;
            }

            var _id = drv.Row.ItemArray[0];
            var _customer_id = drv.Row.ItemArray[1];
            var _header_name = drv.Row.ItemArray[2];
            var _date = drv.Row.ItemArray[3];

// TODO: problem gdy datę wybieram jako pierwszą lub jako ostatnią
            if (_customer_id == System.DBNull.Value || _header_name == System.DBNull.Value || _date == System.DBNull.Value)
            {
                MessageBox.Show("data not fully completed (required: date, header_name, customer_id)");
                return;
            }

            // gdy brakuje klucza głównego => mamy nowy rekord
            if (_id == System.DBNull.Value)
            {
                try
                {
                    var Res = MessageBox.Show("Do you want to Add this header to DB?", "Confirm", MessageBoxButton.YesNo);
                    if (Res == MessageBoxResult.Yes)
                    {
                        string values = String.Format(" VALUES ( '{0}' , '{1}', '{2}' )", _date.ToString(),_customer_id.ToString(), _header_name.ToString());
                        using (SqlConnection con = new SqlConnection(ConString))
                        {
                            SqlCommand crud_command = new SqlCommand();
                            crud_command.Connection = con;
                            con.Open();
                            crud_command.CommandText = "INSERT INTO Headers (date, customer_id, header_name)" + values;
                            crud_command.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show("header added");
            }
            else
            {
                try
                {
                    var Res = MessageBox.Show("Do you want to Update this header in DB?", "Confirm", MessageBoxButton.YesNo);
                    if (Res == MessageBoxResult.Yes)
                    {
                        string values = String.Format(" UPDATE header SET date = '{0}' , customer_id = '{1}' , header_name = '{2}' WHERE id = '{3}' ", _date.ToString(), _customer_id.ToString(), _header_name.ToString(), _id.ToString());
                        using (SqlConnection con = new SqlConnection(ConString))
                        {
                            SqlCommand crud_command = new SqlCommand();
                            crud_command.Connection = con;
                            con.Open();
                            crud_command.CommandText = values;
                            crud_command.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show("header updated");
            }
        }
        
// TODO: pozbyć się tej procedury
        int _index;
        private void dgHeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            _index = dg.SelectedIndex;
            if (_index > 0)
            {
                DataGridRow datarow = dg.ItemContainerGenerator.ContainerFromIndex(_index) as DataGridRow;
            }
        }

        private async void GitRepo()
        {
            const string GitHubIdentity = "jacekstaniec";
            const string GitHubProject = "ITC-WPF";
            GitHubRepoVM gtavm = new GitHubRepoVM(GitHubIdentity, GitHubProject);
            List<GitHubAttribute> myGitHubAttributes = await gtavm.GetGitHubRepoAsync();
            dgGitHub.ItemsSource = null;
            dgGitHub.ItemsSource = myGitHubAttributes;
        }

        private void btn_github_click(object sender, RoutedEventArgs e)
        {
            GitRepo();
        }
    }
}
