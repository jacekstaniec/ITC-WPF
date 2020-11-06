using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Data.SqlClient;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        DataTable dtHeader = new DataTable();
        DataTable dtDetail = new DataTable();
        //    string ConString;

        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }

        void FillDataGrid()
        {
            DBClass.openConnection();

            DBClass.sql = "SELECT Id, Customer_id, Header_name, Date FROM Headers";
            DBClass.cmd.CommandType = CommandType.Text;
            DBClass.cmd.CommandText = DBClass.sql;
            DBClass.daHeader = new SqlDataAdapter(DBClass.cmd);
            DBClass.dtHeader = new DataTable();
            DBClass.daHeader.Fill(DBClass.dtHeader);
            var _keyHeader = new DataColumn[1];
            _keyHeader[0] = DBClass.dtHeader.Columns["Id"];
            DBClass.dtHeader.PrimaryKey = _keyHeader;

            // przy większej liczbie tabel: konieczne rozwiązanie niestatyczne
            DBClass.sql = "SELECT Id, Header_id, Article_name, Quantity, Net, Gross FROM Details";
            DBClass.cmd.CommandType = CommandType.Text;
            DBClass.cmd.CommandText = DBClass.sql;
            DBClass.daDetail = new SqlDataAdapter(DBClass.cmd);
            DBClass.dtDetail = new DataTable();
            DBClass.daDetail.Fill(DBClass.dtDetail);
            var _keyDetail = new DataColumn[1];
            _keyDetail[0] = DBClass.dtDetail.Columns["Id"];
            DBClass.dtDetail.PrimaryKey = _keyDetail;

            dgHeaders.ItemsSource = null;
            dgDetails.ItemsSource = null;
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;

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

        void dgh_NewHeader(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            try
            {
                DBClass.con.Open();
                DBClass.sql = String.Format("INSERT Headers(date,customer_id,header_name) VALUES ( '{0}' , '{1}', '{2}'); SELECT SCOPE_IDENTITY()", DateTime.Now, 0, " ");
                DBClass.cmd.CommandText = DBClass.sql;
                int new_id = Convert.ToInt32(DBClass.cmd.ExecuteScalar());

                DataRow row;
                row = DBClass.dtHeader.NewRow();
                row["Id"] = new_id;
                row["Date"] = DateTime.Now;
                row["Customer_id"] = 0;
                row["Header_name"] = " ";
                DBClass.dtHeader.Rows.Add(row);
                dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
                MessageBox.Show("new (empty) header added");
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.con.Close(); }
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
                    DBClass.sql = String.Format("INSERT Details(header_id,article_name,quantity,net,gross) VALUES ( '{0}' , '{1}', '{2}', '{3}', '{4}'); SELECT SCOPE_IDENTITY()", param.ToString(), "", "0", "0", "0");
                    DBClass.con.Open();
                    DBClass.cmd.CommandText = DBClass.sql;
                    int new_id = Convert.ToInt32(DBClass.cmd.ExecuteScalar());
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
            var param = Convert.ToInt32(button.CommandParameter);  // id

            var Res = MessageBox.Show("Do you want to Delete this header (with articles)", "Confirm", MessageBoxButton.YesNo);
            if (Res == MessageBoxResult.Yes)
            {
                try
                {
                    DataRow row = DBClass.dtHeader.Rows.Find(param);
                    DBClass.dtHeader.Rows.Remove(row);

                    DBClass.con.Open();

                    DBClass.sql = string.Format("DELETE FROM Details WHERE Header_Id = {0}", param.ToString());
                    DBClass.cmd.CommandText = DBClass.sql;
                    DBClass.cmd.ExecuteNonQuery();

                    DBClass.sql = string.Format("DELETE FROM Headers WHERE Id = {0}", param.ToString());
                    DBClass.cmd.CommandText = DBClass.sql;
                    DBClass.cmd.ExecuteNonQuery();

                    MessageBox.Show("header and header's details deleted");
                }
                catch (SqlException ex) { MessageBox.Show(ex.Message); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { DBClass.con.Close(); }

                dgDetails.ItemsSource = null;

                //DataRowView drv = (DataRowView)button.DataContext;
                //int idrv = Convert.ToInt32(drv.Row.ItemArray[0]);     // id

            }
        }

        void dgh_DeleteAttribute(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // id

            DataRowView drv = (DataRowView)button.DataContext;
            int idrv = Convert.ToInt32(drv.Row.ItemArray[1]);     // header_id

            var Res = MessageBox.Show("Do you want to Delete this article", "Confirm", MessageBoxButton.YesNo);
            if (Res == MessageBoxResult.Yes)
            {
                try
                {
                    DataRow row = DBClass.dtDetail.Rows.Find(param);
                    DBClass.dtDetail.Rows.Remove(row);
                    drv.Delete();
                    RefreshDetails(idrv);

                    DBClass.con.Open();

                    DBClass.sql = string.Format("DELETE FROM Details WHERE Id = {0}", param.ToString());
                    DBClass.cmd.CommandText = DBClass.sql;
                    DBClass.cmd.ExecuteNonQuery();

                    MessageBox.Show("article deleted");
                }
                catch (SqlException ex) { MessageBox.Show(ex.Message); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { DBClass.con.Close(); }
            }
        }

        void dgh_SaveChanges(object sender, RoutedEventArgs e)
        {
            dgHeaders.Columns[9].Visibility = Visibility.Hidden;
            int ind = dgHeaders.SelectedIndex;
            DataGridRow editedRow = dgHeaders.ItemContainerGenerator.ContainerFromItem(dgHeaders.Items[ind]) as DataGridRow;
            editedRow.Background = Brushes.White;

            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // id

            DataRowView drv = (DataRowView)button.DataContext;
            string idrv1 = drv.Row.ItemArray[1].ToString();     // customer_id
            string idrv2 = drv.Row.ItemArray[2].ToString();     // header_name
            string idrv3 = drv.Row.ItemArray[3].ToString();     // date
            try
            {
                //                RefreshDetails(idrv);

                DBClass.con.Open();

                DBClass.sql = string.Format("UPDATE Headers set customer_id = '{0}', header_name = '{1}', date = '{2}'  WHERE Id = {3}", idrv1, idrv2, idrv3, param.ToString());
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                MessageBox.Show("header updated");
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.con.Close(); }
            return;
        }

        /// <summary>
        /// event click on github button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_github_click(object sender, RoutedEventArgs e)
        {
            FillGitRepo();
        }

        void dgHeaders_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            dgHeaders.Columns[9].Visibility = Visibility.Hidden;
            int ind = dgHeaders.SelectedIndex;
            DataGridRow editedRow = dgHeaders.ItemContainerGenerator.ContainerFromItem(dgHeaders.Items[ind]) as DataGridRow;
            editedRow.Background = Brushes.Red;
        }

        void dgh_UpdateHeader(object sender, RoutedEventArgs e)
        {
            dgHeaders.Columns[9].Visibility = Visibility.Visible;
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
    }
}
