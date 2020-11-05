using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Data.SqlClient;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        DataTable dtHeader = new DataTable();
        DataTable dtDetail = new DataTable();
        string ConString;

        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }

        void FillDataGrid()
        {
            DBClass.openConnection();

            DBClass.sqlHeader = "SELECT Id, Customer_id, Header_name, Date FROM Headers";
            DBClass.cmdHeader.CommandType = CommandType.Text;
            DBClass.cmdHeader.CommandText = DBClass.sqlHeader;
            DBClass.daHeader = new SqlDataAdapter(DBClass.cmdHeader);
            DBClass.dtHeader = new DataTable();
            DBClass.daHeader.Fill(DBClass.dtHeader);

            DBClass.sqlDetail = "SELECT Id, Header_id, Article_name, Quantity, Net, Gross FROM Details";
            DBClass.cmdDetail.CommandType = CommandType.Text;
            DBClass.cmdDetail.CommandText = DBClass.sqlDetail;
            DBClass.daDetail = new SqlDataAdapter(DBClass.cmdDetail);
            DBClass.dtDetail = new DataTable();
            DBClass.daDetail.Fill(DBClass.dtDetail);

            // TODO: przy wiekszej liczbie tabel - raczej rozwiazanie niestatyczne

            dgHeaders.ItemsSource = null;
            dgDetails.ItemsSource = null;
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
            //                dgDetails.ItemsSource = DBClass.dtDetail.DefaultView;

            DBClass.closeConnection();
        }

        void RefreshDetails(int param)
        {
            ListCollectionView collectionView = new ListCollectionView((System.Collections.IList)DBClass.dtDetail.DefaultView);
            collectionView.Filter = (ff) =>
            {
                DataRowView d = ff as DataRowView;
                if (d != null)
                {
                    if ((Int32)d.Row.ItemArray[1] == param) return true; return false;    // druga kolumna z Details => header_id
                }
                return false;
            };
            dgDetails.ItemsSource = collectionView;
        }

        void dgh_ShowDetails(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            RefreshDetails(param);
        }

        void dgh_NewArticle(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);

            var Res = MessageBox.Show("Do you want to Add new article", "Confirm", MessageBoxButton.YesNo);
            if (Res == MessageBoxResult.Yes)
            {
                try
                {
                    string values = String.Format("INSERT Details(header_id,article_name,quantity,net,gross) VALUES ( '{0}' , '{1}', '{2}', '{3}', '{4}'); SELECT SCOPE_IDENTITY()", param.ToString(), "", "0", "0", "0");
                    SqlCommand crud_cmd = new SqlCommand();
                    crud_cmd.Connection = DBClass.con;
                    crud_cmd.CommandType = CommandType.Text;
                    DBClass.con.Open();
                    crud_cmd.CommandText = values;
                    int new_id = Convert.ToInt32(crud_cmd.ExecuteScalar());

                    DataRow row;
                    row = DBClass.dtDetail.NewRow();
                    row["Id"] = new_id;
                    row["Header_id"] = param;
                    row["Article_name"] = "";
                    row["Quantity"] = "0";
                    row["Net"] = "0";
                    row["Gross"] = "0";
                    DBClass.dtDetail.Rows.Add(row);
                    dgDetails.ItemsSource = DBClass.dtDetail.DefaultView;
                    RefreshDetails(param);
                    MessageBox.Show("new (empty) article added");
                    //TODO: form do uzupełniania danych w locie
                }
                catch (SqlException ex) { MessageBox.Show(ex.Message); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { DBClass.con.Close(); }
            }
        }




        void dgh_DeleteHeader(object sender, RoutedEventArgs e)
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

        void dgh_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
                
        void dgh_AddToDb(object sender, RoutedEventArgs e)
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
        void dgHeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            _index = dg.SelectedIndex;
            if (_index > 0)
            {
                DataGridRow datarow = dg.ItemContainerGenerator.ContainerFromIndex(_index) as DataGridRow;
            }
        }



        async void FillGitRepo()
        {
            const string GitHubIdentity = "jacekstaniec";
            const string GitHubProject = "ITC-WPF";
            GitHubRepoVM gtavm = new GitHubRepoVM(GitHubIdentity, GitHubProject);
            List<GitHubAttribute> myGitHubAttributes = await gtavm.GetGitHubRepoAsync();
            dgGitHub.ItemsSource = null;
            dgGitHub.ItemsSource = myGitHubAttributes;
        }

        /// <summary>
        /// event click on github button (fill git data)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_github_click(object sender, RoutedEventArgs e)
        {
            FillGitRepo();
        }

    }
}
