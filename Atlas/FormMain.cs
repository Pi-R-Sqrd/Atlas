using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Atlas
{
    public partial class FormMain : Form
    {
        private readonly string[] _xmlMimeType = new string[] { "text/xml", "application/xml"};
       
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!IsXMLFile(openFileDialog.SafeFileName))
                    {
                        throw new Exception("The selected file must be a .xml file");
                    }

                    PopulateDataGrid();
                }
                catch (Exception ex)
                {
                    errorProvider.SetError(btnLoadFile, ex.Message);
                }
            }
        }

        private bool IsXMLFile(string fileName)
        {
            var mimetype = GetMimeType(fileName);

            return _xmlMimeType.Contains(mimetype);
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        private void PopulateDataGrid()
        {
            var atlasXML = XElement.Load(openFileDialog.FileName);

            var query = from xmlCountry in atlasXML.Descendants("country")

                        select new
                        {
                            Country = xmlCountry.Attribute("name").Value,
                            City = xmlCountry.Element("city").Attribute("name").Value,
                            Language = xmlCountry.Attribute("languagespoken").Value,
                            Attraction = xmlCountry.Element("city").Attribute("attraction").Value
                        };

            dataGridViewAtlas.DataSource = query.ToList();

        }
    }
}
