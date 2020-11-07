namespace WpfApp1.ViewModels
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows;
    using WpfApp1.Models;

    internal static class RepoManager
    {

        internal static void SetHeaderTable()
        {
            try
            {
                DBClass.openConnection();

                DBClass.sql = String.Format("SELECT Id, Customer_id, Header_name, Date, Net, Gross FROM Headers");
                DBClass.cmd.CommandType = CommandType.Text;
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.daHeader = new SqlDataAdapter(DBClass.cmd);
                DBClass.dtHeader = new DataTable();
                DBClass.daHeader.Fill(DBClass.dtHeader);

                var _keyHeader = new DataColumn[1];
                _keyHeader[0] = DBClass.dtHeader.Columns["Id"];
                DBClass.dtHeader.PrimaryKey = _keyHeader;
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void SetDetailTable()
        {
            try
            {
                DBClass.openConnection();

                DBClass.sql = "SELECT Id, Header_id, Article_name, Quantity, Net, Gross FROM Details";
                DBClass.cmd.CommandType = CommandType.Text;
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.daDetail = new SqlDataAdapter(DBClass.cmd);
                DBClass.dtDetail = new DataTable();
                DBClass.daDetail.Fill(DBClass.dtDetail);

                var _keyDetail = new DataColumn[1];
                _keyDetail[0] = DBClass.dtDetail.Columns["Id"];
                DBClass.dtDetail.PrimaryKey = _keyDetail;
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void RecalcHeaderTable(int header_id)
        {
            const float VAT_23 = 1.23f;
            string expression = "Header_id = " + header_id.ToString();
            DataRow[] rows;
            rows = DBClass.dtDetail.Select(expression);
            float SumNet = 0;
            float SumGross = 0;
            foreach (var item in rows)
            {
                SumNet += Convert.ToSingle(item["Net"]) * Convert.ToInt32(item["quantity"]);
                SumGross += Convert.ToSingle(item["Net"]) * Convert.ToInt32(item["quantity"]) * VAT_23; ;
            }
            try
            {
                DBClass.openConnection();
                DBClass.sql = string.Format("UPDATE Headers SET net='{0}', gross='{1}' WHERE id = '{2}' ", SumNet, SumGross, header_id);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                expression = "Id = " + header_id.ToString();
                rows = DBClass.dtHeader.Select(expression);
                if (rows != null)
                {
                    rows[0]["Net"] = SumNet;
                    rows[0]["Gross"] = SumGross;
                }
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }

        }

        internal static void AddHeader()
        {
            int new_id = 0;
            string now = DateTime.Now.ToString();

            try
            {
                DBClass.openConnection();
                DBClass.sql = String.Format("INSERT Headers(date,customer_id,header_name, net, gross) VALUES ( '{0}' , '{1}', '{2}', '{3}', '{4}'); SELECT SCOPE_IDENTITY()", now, 0, "", 0, 0);
                DBClass.cmd.CommandText = DBClass.sql;
                new_id = Convert.ToInt32(DBClass.cmd.ExecuteScalar());

                DataRow row;
                row = DBClass.dtHeader.NewRow();
                row["Id"] = new_id;
                row["Date"] = now;
                row["Customer_id"] = 0;
                row["Header_name"] = "";
                row["Net"] = 0;
                row["Gross"] = 0;
                DBClass.dtHeader.Rows.Add(row);
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void UpdateHeader(Header header)
        {
            try
            {
                DBClass.openConnection();

                DBClass.sql = string.Format("UPDATE Headers set customer_id = '{0}', header_name = '{1}', date = '{2}'  WHERE Id = {3}", header.Customer_id, header.Header_name, header.Date, header.Id);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }



        internal static void DeleteHeader(int param)
        {
            try
            {
                DBClass.openConnection();

                DBClass.sql = string.Format("DELETE FROM Details WHERE Header_Id = {0}", param);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                DBClass.sql = string.Format("DELETE FROM Headers WHERE Id = {0}", param);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                DataRow row = DBClass.dtHeader.Rows.Find(param);
                var f = DBClass.dtHeader.PrimaryKey;
                DBClass.dtHeader.Rows.Remove(row);
                // dtDetails will be reloaded aftert app restart
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void AddArticle(int param)
        {
            int new_id = 0;
            try
            {
                DBClass.openConnection();

                DBClass.sql = String.Format("INSERT Details(header_id,article_name,quantity,net,gross) VALUES ( '{0}' , '{1}', '{2}', '{3}', '{4}'); SELECT SCOPE_IDENTITY()", param, "", 0, 0, 0);
                DBClass.cmd.CommandText = DBClass.sql;
                new_id = Convert.ToInt32(DBClass.cmd.ExecuteScalar());
                DataRow row;
                row = DBClass.dtDetail.NewRow();
                row["Id"] = new_id;
                row["Header_id"] = param;
                row["Article_name"] = "";
                row["Quantity"] = 0;
                row["Net"] = 0;
                row["Gross"] = 0;
                DBClass.dtDetail.Rows.Add(row);
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void UpdateArticle(Article article)
        {
            const float VAT_23 = 1.23f;
            float _gross = article.Net * article.Quantity * VAT_23;
            try
            {
                DBClass.openConnection();

                DBClass.sql = string.Format("UPDATE Details set header_id ='{0}', article_name='{1}', quantity='{2}', net='{3}', gross='{4}'  WHERE Id ='{5}'", article.Header_id, article.Article_name, article.Quantity, article.Net, _gross, article.Id);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                DataRow row = DBClass.dtDetail.Rows.Find(article.Id);
                row["Gross"] = _gross;
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }

        internal static void DeleteArticle(int param)
        {
            try
            {
                DBClass.openConnection();

                DBClass.sql = string.Format("DELETE FROM Details WHERE Id = {0}", param);
                DBClass.cmd.CommandText = DBClass.sql;
                DBClass.cmd.ExecuteNonQuery();

                DataRow row = DBClass.dtDetail.Rows.Find(param);
                DBClass.dtDetail.Rows.Remove(row);
            }
            catch (SqlException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { DBClass.closeConnection(); }
        }
    }
}