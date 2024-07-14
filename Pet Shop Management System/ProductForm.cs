using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDevHtmlRenderer.Adapters;

namespace Pet_Shop_Management_System
{
    public partial class ProductForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        string title = "Pet Shop Management System";
        public ProductForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            LoadProduct();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProductModule module = new ProductModule(this);
            module.ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                ProductModule module = new ProductModule(this);
                module.lblPcode.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
                module.lblPcode.Visible = true;
                module.txtName.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.cbCategory.Text = dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
                module.txttype.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
                module.dtBDay.Text = dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString();
                if (dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString() == "Male")
                {

                }
                else if (dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString() == "Female")
                {

                }
                module.dtImDay.Text = dgvProduct.Rows[e.RowIndex].Cells[7].Value.ToString();
                module.txtHeight.Text = dgvProduct.Rows[e.RowIndex].Cells[8].Value.ToString();
                module.txtWeight.Text = dgvProduct.Rows[e.RowIndex].Cells[9].Value.ToString();
                module.txtPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[10].Value.ToString();
                module.txtImPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[11].Value.ToString();
                module.txtQty.Text = dgvProduct.Rows[e.RowIndex].Cells[12].Value.ToString();
                //module.txtFileImg.Text = dgvProduct.Rows[e.RowIndex].Cells[13].Value.ToString();
                //FileStream fs;
                //BinaryReader br;
                //string FileName = module.txtFileImg.Text;
                //byte[] ImageData;
                //fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                //br = new BinaryReader(fs);
                //ImageData = br.ReadBytes((int)fs.Length);
                //module.ptImg.Image = ConvertBinaryToImage(ImageData);


                module.btnSave.Enabled = false;
                module.btnUpdate.Enabled = true;
                module.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this items?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbcon.executeQuery("DELETE FROM tbProduct WHERE pcode LIKE '" + dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    MessageBox.Show("Item record has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }            
            else
            {
                txtCategory.Text=dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtNamePet.Text= dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtHeight1.Text= dgvProduct.Rows[e.RowIndex].Cells[8].Value.ToString();
                txtWeight1.Text = dgvProduct.Rows[e.RowIndex].Cells[9].Value.ToString();
                txtSex.Text = dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString();
                DateTime dt1 = DateTime.Parse(dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString()); // tao ra 1 ngay bang ngay sinh
                int moth = 0, day = 0;
                int totalDays1 = Convert.ToInt32((DateTime.UtcNow.Date - dt1.Date).TotalDays); // lay ngay hom nay tru ngay sinh
                if (totalDays1 <= 30)
                {
                     
                     day = totalDays1;
                }
                else
                {
                    moth = totalDays1 / 30;
                    day = totalDays1 % 30;
                }
                txtOld.Text =moth.ToString() ;
                txtOldDay.Text =day.ToString() ;

                //FileStream fs;
                //BinaryReader br;
                //string FileName = dgvProduct.Rows[e.RowIndex].Cells[13].Value.ToString();
                //byte[] ImageData;
                //fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                //br = new BinaryReader(fs);
                //ImageData = br.ReadBytes((int)fs.Length);
                //ptImge.Image = ConvertBinaryToImage(ImageData);
            }
            LoadProduct();

        }

        #region Method

        //Image ConvertBinaryToImage(byte[] data)
        //{
        //    using (MemoryStream ms = new MemoryStream(data))
        //    {
        //        return Image.FromStream(ms);
        //    }
        //}
        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();
            cm = new SqlCommand("SELECT pcode,pname,c.name,ptype,pbday, psex, pimday, pheight, pweight, pprice,pimprice, pqty FROM tbProduct as prod LEFT JOIN tbCategory c ON prod.pcategory = c.cateid WHERE CONCAT(pcode,pname,ptype,pcategory) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), DateTime.Parse(dr[4].ToString()).ToShortDateString(),
                    dr[5].ToString(), DateTime.Parse(dr[6].ToString()).ToShortDateString(), dr[7].ToString(), dr[8].ToString(), dr[9].ToString(), dr[10].ToString(),
                    dr[11].ToString());
            }
            dr.Close();
            cn.Close();
        }
        #endregion Method
    }
}
