using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class CashForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        string title = "Pet Shop Management System";
        MainForm main;
       
        public CashForm(MainForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            main = form;
            loadDailySale();
            getTransno();
            loadCash();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CashProduct product = new CashProduct(this);
            product.uname = main.lblUsername.Text;
            lblIdc.Text = main.lblID.Text;
            product.ShowDialog();
            
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            CashCustomer customer = new CashCustomer(this);
            customer.ShowDialog();

            if(MessageBox.Show("Are you sure you want to cash this product?","Cashing",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                customer.ChoiceCustomer(lblCidca.Text);
                CreateBillDeatil();
                
                for (int i=0; i<dgvCash.Rows.Count; i++)
                {
                    dbcon.executeQuery("UPDATE tbProduct SET pqty= pqty - " + int.Parse(dgvCash.Rows[i].Cells[4].Value.ToString()) + " WHERE pcode LIKE " + dgvCash.Rows[i].Cells[2].Value.ToString() + "");
                }
                loadCash();
                getTransno();
                loadDailySale();

                //dgvCash.Rows.Clear();
            }
            else
            {
                loadCash();
            }
        }

       

        private void dgvCash_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCash.Columns[e.ColumnIndex].Name;
            removeitem:
            if(colName=="Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this cash?", "Delete Cash", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbcon.executeQuery("DELETE FROM tbCash WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    MessageBox.Show("Cash record has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }               
            }
            else if (colName == "Increase")
            {
                int i = checkPqty(dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString());
                if(int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString()) < i)
                {
                    dbcon.executeQuery("UPDATE tbCash SET qty = qty + " + 1 + " WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                }
               else
                {
                    MessageBox.Show("Remaining quantity on hand is " + i + "!", "Out of Stock ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (colName == "Decrease")
            {
                if(int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString()) == 1)
                {
                    colName = "Delete";
                    goto removeitem;
                }
                dbcon.executeQuery("UPDATE tbCash SET qty = qty - " + 1 + " WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
            }
            loadCash();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            progress.Invoke((MethodInvoker)delegate
            {
                progress.Text = DateTime.Now.ToString("hh:mm:ss");
                progress.Value = Convert.ToInt32(DateTime.Now.Second);
            });
        }

        #region method
        public void getTransno()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                int count;
                string transno;

                cn.Open();
                cm = new SqlCommand("SELECT TOP 1 transno FROM tbCash WHERE transno LIKE '" + sdate + "%' ORDER BY cashid DESC", cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    transno = dr[0].ToString();
                    count = int.Parse(transno.Substring(8, 4));
                    lblTransno.Text = sdate + (count + 1);
                }
                else
                {
                    transno = sdate + "1001";
                    lblTransno.Text = transno;
                }
                dr.Close();
                cn.Close();

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }


        public void loadCash()
        {
            try
            {
                int i = 0;
                double total = 0;
                dgvCash.Rows.Clear();
                cm = new SqlCommand("SELECT cashid,pcode,pname,qty,price,total,c.name,cashier FROM tbCash as cash LEFT JOIN tbCustomer c ON cash.cid = c.id WHERE transno LIKE " + lblTransno.Text + "", cn);
                cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvCash.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                    total += double.Parse(dr[5].ToString());
                }
                dr.Close();
                cn.Close();
                lblTotal.Text = total.ToString("#,##0");
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        public  int checkPqty(string pcode)
        {
            int i = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT pqty FROM tbProduct WHERE pcode LIKE '" + pcode + "'", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
            return i;
        }
        public bool checkPcode(string pcode)
        {
            bool flag = false;
            cn.Open();
            string sql = "SELECT pcode FROM tbCash WHERE transno='" + lblTransno.Text.Trim() + "' and pcode='" + pcode + "'";
            SqlDataAdapter dap = new SqlDataAdapter(sql, cn);
            DataTable table = new DataTable();
            dap.Fill(table);
            if (table.Rows.Count > 0)
                flag = true;
            cn.Close();
            return flag;
        }
        public void loadDailySale()
        {
            string sdate = DateTime.Now.ToString("yyyyMMdd");

            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT ISNULL(SUM(total),0) AS total FROM tbCash WHERE transno LIKE'" + sdate + "%'", cn);
                lblDailySale.Text = double.Parse(cm.ExecuteScalar().ToString()).ToString("#,##0");
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void CreateBillDeatil()
        {
            cm = new SqlCommand("INSERT INTO tbBillDetail(transno, cid, cashier,createday) VALUES (@transno, @cid, @cashier, @createday)", cn);

            cm.Parameters.AddWithValue("@transno", lblTransno.Text);
            cm.Parameters.AddWithValue("@cid", lblCidca.Text);
            cm.Parameters.AddWithValue("@cashier", lblIdc.Text);
            cm.Parameters.AddWithValue("@createday", DateTime.Now.ToString());

            cn.Open();
            cm.ExecuteNonQuery();
            cn.Close();
        }
        

        #endregion method

        private void CashForm_Load(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("INVOICE", new Font("Arial", 20, FontStyle.Bold), Brushes.Orange, new Point(250, 10));
            Font print10B = new Font("Lucida Console", 10, FontStyle.Bold);
            Font print8B = new Font("Arial", 9, FontStyle.Regular);
            Font print6B = new Font("Arial", 8, FontStyle.Regular);

            e.Graphics.DrawString("   PETSHOP88   ", print10B, Brushes.Black, 240, 50);
            e.Graphics.DrawString("--------------------------------------------------------------------------------", new Font(dgvCash.Font.FontFamily, 8, FontStyle.Regular), Brushes.Black, 250, 70);
            e.Graphics.DrawString("Produs         | Quantity | price               | Total          ", print8B, Brushes.Black, 250, 100);
            int height = 0;
            int x = 250;
            for (int i = 0; i < dgvCash.Rows.Count; i++)
            {
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["column3"].Value.ToString(), print6B, Brushes.Black, x, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["column6"].Value.ToString(), print6B, Brushes.Black, x + 95, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["column7"].Value.ToString(), print6B, Brushes.Black, x + 140, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["column7"].Value.ToString(), print6B, Brushes.Black, x + 140 + 85, 120 + height);
                height += 20;
            }
            e.Graphics.DrawString("------------------------------------------------------------------------------", new Font(dgvCash.Font.FontFamily, 8, FontStyle.Regular), Brushes.Black, 250, 120 + height);
            e.Graphics.DrawString("Total:  " + lblTotal.Text + " VND", print10B, Brushes.Black, 340, 140 + height);

        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                printPreviewDialog.Document = printDocument;
                pageSetupDialog.PageSettings = printDocument.DefaultPageSettings;
                pageSetupDialog.ShowDialog();
                printDocument.DefaultPageSettings = pageSetupDialog.PageSettings;
                printPreviewDialog.ShowDialog();
            }
            catch
            {
                MessageBox.Show("error");
            }
        }
    }
}