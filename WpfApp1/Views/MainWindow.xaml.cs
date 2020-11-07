using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WpfApp1.Models;
using WpfApp1.ViewModels;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }

        void FillDataGrid()
        {
            RepoManager.SetHeaderTable();
            RepoManager.SetDetailTable();

            dgHeaders.ItemsSource = null;
            dgDetails.ItemsSource = null;
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
        }

        #region click button events

        void dg_ShowArticles(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            RefreshDetailsGrid(param);
        }

        void dg_AddHeader(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            RepoManager.AddHeader();
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
            MessageBox.Show("new (empty) header added");
        }

        void dg_AddArticle(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            var response = MessageBox.Show("Do you want to Add a new article", "Confirm", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                RepoManager.AddArticle(param);
                dgDetails.ItemsSource = DBClass.dtDetail.DefaultView;
                RefreshDetailsGrid(param);
                MessageBox.Show("new (empty) article added");
            }
        }

        void dg_DeleteHeader(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // id
            var response = MessageBox.Show("Do you want to Delete this header (with all articles)", "Confirm", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                RepoManager.DeleteHeader(param);
                dgDetails.ItemsSource = null;
                dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;               //refresh
                MessageBox.Show("the header and all header's articles deleted");
            }
        }

        void dg_DeleteArticle(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // article_id
            DataRowView drv = (DataRowView)button.DataContext;
            int header_id = Convert.ToInt32(drv.Row.ItemArray[1]);  // header_id

            var response = MessageBox.Show("Do you want to Delete this article", "Confirm", MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
            {
                RepoManager.DeleteArticle(param);            // delete from db, delete from table
                RepoManager.RecalcHeaderTable(header_id);    // recalc db, recal head tale
                RefreshDetailsGrid(header_id);               // refresh detail_grid
                MessageBox.Show("article deleted");
            }
        }

        void dg_UpdateHeader(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // id

            DataRowView drv = (DataRowView)button.DataContext;
            var i0 = drv.Row.ItemArray[0];                   // id
            var i1 = drv.Row.ItemArray[1];                   // customer_id
            var i2 = drv.Row.ItemArray[2];                   // header_name
            var i3 = drv.Row.ItemArray[3];                   // date
            float i4 = Convert.ToSingle(drv.Row.ItemArray[4]);                   // net
            float i5 = Convert.ToSingle(drv.Row.ItemArray[5]);                   // gross

            Header header = new Header((int)i0, (DateTime)i3, (string)i2, (int)i1, i4, i5);
            RepoManager.UpdateHeader(header);
            RowRedWhiteMethod(dgHeaders, Brushes.White);
            MessageBox.Show("header updated");
        }

        void dg_UpdateAttribute(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
            var button = sender as Button;
            var param = Convert.ToInt32(button.CommandParameter);  // id

            DataRowView drv = (DataRowView)button.DataContext;
            var i0 = drv.Row.ItemArray[0];                   // id
            var i1 = drv.Row.ItemArray[1];                   // header_id
            var i2 = drv.Row.ItemArray[2];                   // article_name
            var i3 = drv.Row.ItemArray[3];                   // quantity
            float i4 = Convert.ToSingle(drv.Row.ItemArray[4]);                   // net
            float i5 = 0.0f; // will be recalculated

            Article article = new Article((int)i0, (int)i1, (string)i2, (int)i3, (float)i4, (float)i5);
            RepoManager.UpdateArticle(article);                     // update db, update table table  //TODO: separate?
            RepoManager.RecalcHeaderTable(Convert.ToInt32(i1));     // recalc db, recalc table
            RowRedWhiteMethod(dgDetails, Brushes.White);
            MessageBox.Show("article updated");
        }

        void dg_EnableUpdateColumns(object sender, RoutedEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Visible);
        }

        #endregion




        #region row edit begining/ending triggers

        void dgDetails_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            GridUpdateColumnsVisibilityTrick(Visibility.Hidden);
        }

        void dgHeaders_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) { RowRedWhiteMethod(dgHeaders, Brushes.Red); }

        void dgDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) { RowRedWhiteMethod(dgDetails, Brushes.Red); }

        #endregion



        /// <summary>
        /// github button event click 
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">routed event args</param>
        void btn_github_click(object sender, RoutedEventArgs e) { FillGitRepoAsync(); }

        void RefreshDetailsGrid(int param)
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
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
            dgDetails.ItemsSource = collectionView;
        }



        void RowRedWhiteMethod(DataGrid dg, SolidColorBrush colorBrush)
        {
            int ind = dg.SelectedIndex;
            DataGridRow editedRow = dg.ItemContainerGenerator.ContainerFromItem(dg.Items[ind]) as DataGridRow;
            editedRow.Background = colorBrush;
            //            editedRow.Background = Brushes.Red;
        }

        /// <summary>
        /// Trick method necessary for delayed grid functionality 
        /// </summary>
        /// <param name="v">visibility</param>
        private void GridUpdateColumnsVisibilityTrick(Visibility v)
        {
            int headersGridUpdateColumn = 9;
            int detailsGridUpdateColumn = 7;
            dgHeaders.Columns[headersGridUpdateColumn].Visibility = v;
            dgDetails.Columns[detailsGridUpdateColumn].Visibility = v;
        }

        async void FillGitRepoAsync()
        {
            const string GitHubIdentity = "jacekstaniec";
            const string GitHubProject = "ITC-WPF";
            GitHubRepo gtavm = new GitHubRepo(GitHubIdentity, GitHubProject);
            List<GitHub> myGitHubAttributes = await gtavm.GetGitHubRepoAsync();
            dgGitHub.ItemsSource = null;
            dgGitHub.ItemsSource = myGitHubAttributes;
        }
    }
}