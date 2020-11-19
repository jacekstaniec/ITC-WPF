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

        /// <summary>
        /// fill data grids
        /// </summary>
        void FillDataGrid()
        {
            RepoManager.SetHeaderTable();
            RepoManager.SetDetailTable();

            dgHeaders.ItemsSource = null;
            dgDetails.ItemsSource = null;
            dgHeaders.ItemsSource = DBClass.dtHeader.DefaultView;
        }



        #region click button events TODO: replace all by ICommands (view-model)

        /// <summary>
        /// show related articles event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_ShowArticles(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var param = Convert.ToInt16(button.CommandParameter);
            RefreshDetailsGrid(param);
        }

        /// <summary>
        /// replaced by AddHeaderCommand command (in view-model)
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_AddHeader(object sender, RoutedEventArgs e)
        {
            // code replaced by AddHeaderCommand command (in view-model)
        }

        /// <summary>
        /// add article event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_AddArticle(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// delate header event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_DeleteHeader(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// delete article event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_DeleteArticle(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// update header event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_UpdateHeader(object sender, RoutedEventArgs e)
        {
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
            MessageBox.Show("header updated");
        }

        /// <summary>
        /// update article event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void dg_UpdateAttribute(object sender, RoutedEventArgs e)
        {
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
            MessageBox.Show("article updated");
        }

        #endregion

        /// <summary>
        /// github refresh atributes button event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event-args</param>
        void btn_github_click(object sender, RoutedEventArgs e) { FillGitRepoAsync(); }

        /// <summary>
        /// refresh grid with artickes method
        /// </summary>
        /// <param name="param">article_id</param>
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

        /// <summary>
        /// gets some data from github api
        /// </summary>
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