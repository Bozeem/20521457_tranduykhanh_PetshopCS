using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class ProductModule : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlConnection cn1 = new SqlConnection();
        SqlCommand cm1 = new SqlCommand();

        DbConnect dbcon = new DbConnect();
        string title = "Pet Shop Management System";
        bool check = false;
        ProductForm product;
        public ProductModule(ProductForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            cn1 = new SqlConnection(dbcon.connection());
            product = form;
            
            LoadCombobox();
            cbCategory.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CheckField();
                if (check == true)
                {
                    if (MessageBox.Show("Are you sure you want to register this product?", "Product Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("INSERT INTO tbProduct(pname,  pcategory, ptype,pbday, psex, pimday, pheight, pweight, pprice,pimprice, pqty)VALUES(@pname,  @pcategory, @ptype,@pbday, @psex, @pimday, @pheight, @pweight, @pprice,@pimprice, @pqty)", cn);
                        cm.CommandType = CommandType.Text;                      
                        int petcategory = Convert.ToInt32(cbCategory.SelectedValue.ToString());
                        cm.Parameters.AddWithValue("@pname", txtName.Text);
                        cm.Parameters.AddWithValue("@pcategory",petcategory);
                        cm.Parameters.AddWithValue("@ptype", txttype.Text);
                        cm.Parameters.AddWithValue("@pbday", dtBDay.Value);
                        if (rdMale.Checked == true)
                        {
                            cm.Parameters.AddWithValue("@psex", "Male");
                        }
                        else if (rdFemale.Checked == true)
                        {
                            cm.Parameters.AddWithValue("@psex", "Female");
                        }

                        cm.Parameters.AddWithValue("@pimday", dtImDay.Value);
                        cm.Parameters.AddWithValue("@pheight", double.Parse(txtHeight.Text));
                        cm.Parameters.AddWithValue("@pweight", double.Parse(txtWeight.Text));
                        cm.Parameters.AddWithValue("@pprice", double.Parse(txtPrice.Text));
                        cm.Parameters.AddWithValue("@pimprice", double.Parse(txtImPrice.Text));
                        cm.Parameters.AddWithValue("@pqty", int.Parse(txtQty.Text));
                        //cm.Parameters.AddWithValue("@pimg", txtFileImg.Text);

                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();

                        
                        MessageBox.Show("Product has been successfully registered!", title);
                        Clear();
                        product.LoadProduct();
                    }

                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                CheckField();
                if (check)
                {
                    if (MessageBox.Show("Are you sure you want to Edit this product?", "Product Edited", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("UPDATE tbProduct SET pname=@pname,  pcategory=@pcategory, ptype=@ptype,pbday=@pbday, psex=@psex, pimday=@pimday, pheight=@pheight, pweight=@pweight, pprice=@pprice,pimprice=@pimprice, pqty=@pqty WHERE pcode=@pcode", cn);
                        cm.CommandType = CommandType.Text;
                        int petcategory = Convert.ToInt32(cbCategory.SelectedValue.ToString());
                        cm.Parameters.AddWithValue("@pcode", lblPcode.Text);
                        cm.Parameters.AddWithValue("@pname", txtName.Text);
                        cm.Parameters.AddWithValue("@pcategory", petcategory);
                        cm.Parameters.AddWithValue("@ptype", txttype.Text);
                        cm.Parameters.AddWithValue("@pbday", dtBDay.Value);
                        if (rdMale.Checked == true)
                        {
                            cm.Parameters.AddWithValue("@psex", "Male");
                        }
                        else if (rdFemale.Checked == true)
                        {
                            cm.Parameters.AddWithValue("@psex", "Female");
                        }

                        cm.Parameters.AddWithValue("@pimday", dtImDay.Value);
                        cm.Parameters.AddWithValue("@pheight", double.Parse(txtHeight.Text));
                        cm.Parameters.AddWithValue("@pweight", double.Parse(txtWeight.Text));
                        cm.Parameters.AddWithValue("@pprice", double.Parse(txtPrice.Text));
                        cm.Parameters.AddWithValue("@pimprice", double.Parse(txtImPrice.Text));
                        cm.Parameters.AddWithValue("@pqty", int.Parse(txtQty.Text));
                        //cm.Parameters.AddWithValue("@pimg", txtFileImg.Text);
                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Product has been successfully updated!", title);
                        product.LoadProduct();
                        this.Dispose();
                    }

                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }


        private void txtQty_KeyPress(object sender, KeyPressEventArgs e) // cay rang buoc chi nhap so
        {
            // only allow digit number 
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e) // cay rang buoc chi nhap so
        {
            // only allow digit number 
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        #region Method
        public void Clear()
        {
            txtName.Clear();
            txttype.Clear();
            dtBDay.Value = DateTime.Now;
            dtImDay.Value = DateTime.Now;

            txtHeight.Clear();
            txtWeight.Clear();
            txtPrice.Clear();
            txtQty.Clear();
            txtImPrice.Clear();
            cbCategory.SelectedIndex = 0;

            
            rdFemale.Checked = false;
            rdMale.Checked = false;

            
        }

        public void CheckField()
        {
            double price = double.Parse(txtPrice.Text);
            double imprice = double.Parse(txtImPrice.Text);
            double press = imprice-price;
            if (txtName.Text == "" | txtPrice.Text == "" | txtQty.Text == "" | txttype.Text == "" | txtHeight.Text == "" | txtWeight.Text=="" | txtQty.Text=="" | txtImPrice.Text=="" | press>0)
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }
       public void LoadCombobox() // load danh muc thu cung len combobox 
        {
            cn1.Open();
            cm1 = new SqlCommand("SELECT cateid,name FROM tbCategory", cn1);
            SqlDataReader dr1 = cm1.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(dr1);          
            cbCategory.ValueMember = "cateid";
            cbCategory.DisplayMember = "name";
            cbCategory.DataSource = dt1;
            dr1.Close();
            cn1.Close();

        }

        #endregion Method

        private void btnImg_Click(object sender, EventArgs e)
        {
            //string FileName = null;

            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.RestoreDirectory = true;

            //openFileDialog.Filter = "All picture files (*.BMP;*.JPG;*.JPEG;*.PNG;*.GIF)|*.BMP;*.JPG;*.JPEG;*.PNG;*.GIF";

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    FileName = openFileDialog.FileName;
            //    ptImg.Image = Image.FromFile(FileName);
            //    txtFileImg.Text = FileName;

            //}
        }

        private void txtImPrice_KeyPress(object sender, KeyPressEventArgs e) // cay rang buoc chi nhap so
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e) // cay rang buoc chi nhap so
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
