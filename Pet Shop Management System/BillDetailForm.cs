using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{

    public partial class BillDetailForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        
        
        MainForm main;
        string title = "Pet Shop Management System";
        public BillDetailForm(MainForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            main = form;           
            loadCash();
        }
        public void loadCash()
        {
            try
            {
               int i = 0;                
               dgvBillDetail.Rows.Clear();
               cm = new SqlCommand("SELECT billid,A.transno,cid,cashier,total,createday  FROM (SELECT billid,transno,cid,cashier,createday FROM tbBillDetail) AS A INNER JOIN (SELECT transno,SUM(total) AS total FROM tbCash GROUP BY transno) AS B ON A.transno = B.transno", cn);
               
               cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvBillDetail.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString());
                   
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
       
        private void dgvBillDetail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvBillDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dgvBillDetail.CurrentRow;
            lblTransn.Text = row.Cells["Column10"].Value.ToString();
            loadCashBill(row.Cells["Column10"].Value.ToString());
            


        }
        #region method
        public void loadCashBill(string transno)
        {
            try
            {
                int i = 0;
                double total = 0;
                dgvCash.Rows.Clear();
                cm = new SqlCommand("SELECT cashid,pcode,pname,qty,price,total,c.name,cashier FROM tbCash as cash LEFT JOIN tbCustomer c ON cash.cid = c.id WHERE transno = '" + transno + "'", cn);
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
                lblTotal1.Text = total.ToString("#,##0");
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }
        private void ToExcel(DataGridView dataGridView1, string fileName)
        {
            
        }
        #endregion method

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                dgvBillDetail.Rows.Clear();
                cm = new SqlCommand("SELECT billid,A.transno,cid,cashier,total,createday  FROM (SELECT billid,transno,cid,cashier,createday FROM tbBillDetail) AS A INNER JOIN (SELECT transno,SUM(total) AS total FROM tbCash GROUP BY transno) AS B ON A.transno = B.transno WHERE createday BETWEEN '"+dtFrom.Text+"' AND '" + dtTo.Text + "' ", cn);

                cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvBillDetail.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString());

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

        private void btnExport_Click(object sender, EventArgs e)
        {

            if (dgvBillDetail.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application XcelApp = new Microsoft.Office.Interop.Excel.Application();
                XcelApp.Application.Workbooks.Add(Type.Missing);

                for (int i = 1; i < dgvBillDetail.Columns.Count + 1; i++)
                {
                    XcelApp.Cells[1, i] = dgvBillDetail.Columns[i - 1].HeaderText;
                }

                for (int i = 0; i < dgvBillDetail.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvBillDetail.Columns.Count; j++)
                    {
                        XcelApp.Cells[i + 2, j + 1] = dgvBillDetail.Rows[i].Cells[j].Value.ToString();
                    }
                }
                XcelApp.Columns.AutoFit();
                XcelApp.Visible = true;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                printPreviewDialog1.Document = printDocument1;
                pageSetupDialog1.PageSettings = printDocument1.DefaultPageSettings;
                pageSetupDialog1.ShowDialog();
                printDocument1.DefaultPageSettings = pageSetupDialog1.PageSettings;
                printPreviewDialog1.ShowDialog();
            }
            catch
            {
                MessageBox.Show("error");
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
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
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["Product"].Value.ToString(), print6B, Brushes.Black, x, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["Quantity"].Value.ToString(), print6B, Brushes.Black, x + 95, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["Price"].Value.ToString(), print6B, Brushes.Black, x + 140, 120 + height);
                e.Graphics.DrawString(dgvCash.Rows[i].Cells["Total"].Value.ToString(), print6B, Brushes.Black, x + 140 + 85, 120 + height);
                height += 20;
            }
            e.Graphics.DrawString("------------------------------------------------------------------------------", new Font(dgvCash.Font.FontFamily, 8, FontStyle.Regular), Brushes.Black, 250, 120 + height);
            e.Graphics.DrawString("Total:  " + lblTotal1.Text + " VND", print10B, Brushes.Black, 340, 140+height);

        }
    }
}
