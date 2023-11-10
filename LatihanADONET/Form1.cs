using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;

namespace LatihanADONET
{
    public partial class Form1 : Form
    {
        // constructor
        public Form1()
        {
            InitializeComponent();
            InisialisasiListView();
        }

        private SQLiteConnection GetOpenConnection()
        {
            SQLiteConnection conn = null;
            try
            {
                string dbName = @"D:\Database\DbPerpustakaan.db";

                string connectionString = $"Data Source={dbName};Version=3;";

                conn = new SQLiteConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return conn;
        }

        private void btnTesKoneksi_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = GetOpenConnection();

            if (conn != null && conn.State == ConnectionState.Open)
            {
                MessageBox.Show("Koneksi ke database berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conn.Close(); // Close the connection after testing
            }
            else
            {
                MessageBox.Show("Koneksi ke database gagal!!!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void InisialisasiListView()
        {
            lvwMahasiswa.View = View.Details;
            lvwMahasiswa.FullRowSelect = true;
            lvwMahasiswa.GridLines = true;
            lvwMahasiswa.Columns.Add("No.", 30, HorizontalAlignment.Center);
            lvwMahasiswa.Columns.Add("NPM", 70, HorizontalAlignment.Center);
            lvwMahasiswa.Columns.Add("Nama", 190, HorizontalAlignment.Left);
            lvwMahasiswa.Columns.Add("Angkatan", 70, HorizontalAlignment.Center);
        }

        private void btnTampilkanData_Click(object sender, EventArgs e)
        {
            lvwMahasiswa.Items.Clear();
            SQLiteConnection conn = GetOpenConnection();

            string sql = @"select npm, nama, angkatan 
                            from mahasiswa 
                            order by nama";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                var noUrut = lvwMahasiswa.Items.Count + 1;
                var item = new ListViewItem(noUrut.ToString());
                item.SubItems.Add(dtr["npm"].ToString());
                item.SubItems.Add(dtr["nama"].ToString());
                item.SubItems.Add(dtr["angkatan"].ToString());
                lvwMahasiswa.Items.Add(item);

            }
            dtr.Dispose();
            cmd.Dispose();
            conn.Dispose();

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var result = 0;
            if (String.IsNullOrEmpty(txtNpmInsert.Text))
            {
                MessageBox.Show("NPM harus diisi !!!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNpmInsert.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtNamaInsert.Text))
            {
                MessageBox.Show("Nama harus diisi !!!", "Informasi",
               MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                txtNamaInsert.Focus();
                return;
            }
            SQLiteConnection conn = GetOpenConnection();
            var sql = @"insert into mahasiswa (npm, nama, angkatan)
                        values (@npm, @nama, @angkatan)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            try
            {
                cmd.Parameters.AddWithValue("@npm", txtNpmInsert.Text);
                cmd.Parameters.AddWithValue("@nama", txtNamaInsert.Text);
                cmd.Parameters.AddWithValue("@angkatan", txtAngkatanInsert.Text);

                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cmd.Dispose();
            }

            if (result > 0)
            {
                MessageBox.Show("Data mahasiswa berhasil disimpan !",
               "Informasi", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                txtNpmInsert.Clear();
                txtNamaInsert.Clear();
                txtAngkatanInsert.Clear();
                txtNpmInsert.Focus();

            }
            else
                MessageBox.Show("Data mahasiswa gagal disimpan !!!",
               "Informasi", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            conn.Dispose();
        }
       

        private void btnCariUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNpmUpdate.Text))
            {
                MessageBox.Show("NPM harus diisi!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNpmUpdate.Focus();
                return;
            }

            SQLiteConnection conn = GetOpenConnection();
            string sql = @"select npm, nama, angkatan from mahasiswa where npm = @npm";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@npm", txtNpmUpdate.Text);

            SQLiteDataReader dtr = cmd.ExecuteReader();

            if (dtr.Read())
            {
                txtNpmUpdate.Text = dtr["npm"].ToString();
                txtNamaUpdate.Text = dtr["nama"].ToString();
                txtAngkatanUpdate.Text = dtr["angkatan"].ToString();
            }
            else
            {
                MessageBox.Show("Data mahasiswa tidak ditemukan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            dtr.Close(); // Close the data reader
            cmd.Dispose();
            conn.Close(); // Close the connection
            conn.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var result = 0;
            if (string.IsNullOrEmpty(txtNpmUpdate.Text))
            {
                MessageBox.Show("NPM harus diisi!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNpmUpdate.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtNamaUpdate.Text))
            {
                MessageBox.Show("Nama harus diisi!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNamaUpdate.Focus();
                return;
            }

            SQLiteConnection conn = GetOpenConnection();

            string sql = @"update mahasiswa set nama = @nama, angkatan = @angkatan where npm = @npm";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            try
            {
                cmd.Parameters.AddWithValue("@nama", txtNamaUpdate.Text);
                cmd.Parameters.AddWithValue("@angkatan", txtAngkatanUpdate.Text);
                cmd.Parameters.AddWithValue("@npm", txtNpmUpdate.Text);

                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cmd.Dispose();
    
            }

            if (result > 0)
            {
                MessageBox.Show("Data mahasiswa berhasil diupdate!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // reset form
                txtNpmUpdate.Clear();
                txtNamaUpdate.Clear();
                txtAngkatanUpdate.Clear();
                txtNpmUpdate.Focus();
            }
            else
            {
                MessageBox.Show("Data mahasiswa gagal diupdate!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            conn.Close(); // Close the connection
            conn.Dispose();


        }
        private void btnCariDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNpmDelete.Text))
            {
                MessageBox.Show("NPM harus diisi!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNpmDelete.Focus();
                return;
            }

            SQLiteConnection conn = GetOpenConnection();
            string sql = @"select npm, nama, angkatan from mahasiswa where npm = @npm";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@npm", txtNpmDelete.Text);

            SQLiteDataReader dtr = cmd.ExecuteReader();

            if (dtr.Read())
            {
                txtNpmDelete.Text = dtr["npm"].ToString();
                txtNamaDelete.Text = dtr["nama"].ToString();
                txtAngkatanDelete.Text = dtr["angkatan"].ToString();
            }
            else
            {
                MessageBox.Show("Data mahasiswa tidak ditemukan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            dtr.Close(); // Close the data reader
            cmd.Dispose();
            conn.Close(); // Close the connection
            conn.Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNpmDelete.Text))
            {
                MessageBox.Show("NPM harus diisi!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNpmDelete.Focus();
                return;
            }

            SQLiteConnection conn = GetOpenConnection();
            string sql = @"DELETE FROM mahasiswa WHERE npm = @npm";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@npm", txtNpmDelete.Text);

            int result = cmd.ExecuteNonQuery();

            if (result > 0)
            {
                MessageBox.Show("Data mahasiswa berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Reset form or update UI if needed
                txtNpmDelete.Clear();
                // Clear other fields if any
            }
            else
            {
                MessageBox.Show("Data mahasiswa gagal dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            cmd.Dispose();
            conn.Close();
            conn.Dispose();
        }


    }
}
