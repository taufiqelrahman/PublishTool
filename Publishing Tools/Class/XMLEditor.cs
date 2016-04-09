using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Publishing_Tools.Class
{
    class XMLEditor
    {
        XmlDocument xml;

        public void SearchAtt(TextBox xmlPath, TextBox structureBox, TextBox keyBox, TextBox currentValueText, DataGridView dgvConfig, 
            Button updateValueButton,  Button addXMLButton, TextBox newKeyText, TextBox newValueText, string attName)
        {
            try
            {
                //read xml file
                //po = XDocument.Load(xmlPath.Text);
                xml = new XmlDocument();
                //xml.LoadXml(po.ToString());
                xml.Load(xmlPath.Text);
                XmlNodeList nodes = xml.SelectNodes(structureBox.Text);
                string[] addConfig;

                foreach (XmlNode node in nodes)
                {
                    XmlAttributeCollection nodeAtt = node.Attributes;
                    if (nodeAtt[attName].Value.ToLower().ToString() == keyBox.Text.ToLower() || nodeAtt[attName].Value.ToLower().ToString().Contains(keyBox.Text.ToLower()))
                    {
                        currentValueText.Text = nodeAtt[attName].Value.ToLower().ToString();
                        addConfig = new string[] { nodeAtt["key"].Value.ToString(), nodeAtt["value"].Value.ToString() };
                        dgvConfig.Rows.Add(addConfig);
                        updateValueButton.Enabled = true;
                    }
                }

                if (string.IsNullOrEmpty(currentValueText.Text))
                {
                    currentValueText.Text = "Key belum ada.";
                    MessageBox.Show("Key belum ada.");
                    addXMLButton.Enabled = true;
                    newKeyText.Enabled = true;
                    newValueText.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void SearchAtt(TextBox xmlPath, TextBox structureBox, TextBox keyBox, TextBox currentValueText, DataGridView dgvConfig,
            Button updateValueButton, Button addXMLButton, TextBox newNameText, TextBox newConnStringText, TextBox newProvNameText, 
            string attName)
        {
            try
            {
                //read xml file
                //po = XDocument.Load(xmlPath.Text);
                xml = new XmlDocument();
                //xml.LoadXml(po.ToString());
                xml.Load(xmlPath.Text);
                XmlNodeList nodes = xml.SelectNodes(structureBox.Text);
                string[] addConfig;

                foreach (XmlNode node in nodes)
                {
                    XmlAttributeCollection nodeAtt = node.Attributes;
                    if (nodeAtt[attName].Value.ToLower().ToString() == keyBox.Text.ToLower() || nodeAtt[attName].Value.ToLower().ToString().Contains(keyBox.Text.ToLower()))
                    {
                        currentValueText.Text = nodeAtt[attName].Value.ToLower().ToString();
                        addConfig = new string[] { nodeAtt["name"].Value.ToString(), nodeAtt["connectionString"].Value.ToString(), 
                        nodeAtt["providerName"].Value.ToString() };
                        dgvConfig.Rows.Add(addConfig);
                        updateValueButton.Enabled = true;
                    }
                }

                if (string.IsNullOrEmpty(currentValueText.Text))
                {
                    currentValueText.Text = attName + " belum ada.";
                    MessageBox.Show(attName + " belum ada.");
                    addXMLButton.Enabled = true;
                    newNameText.Enabled = true;
                    newConnStringText.Enabled = true;
                    newProvNameText.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void UpdateXML(DataGridView dgvConfig, TextBox structureBox, TextBox xmlPath, ToolStripStatusLabel lastActivity, string att1,
            string att2)
        {
            try
            {
                for (int i = 0; i < dgvConfig.Rows.Count - 1; i++)
                {
                    //updateXML(, );
                    XmlNode node = xml.SelectSingleNode(structureBox.Text + "[@" + att1 + "='" + dgvConfig.Rows[i].Cells[0].Value.ToString() + "']");
                    node.Attributes[att2].Value = dgvConfig.Rows[i].Cells[1].Value.ToString();
                    xml.Save(xmlPath.Text);
                }
                MessageBox.Show(dgvConfig.Rows.Count - 1 + " " + att1 + " telah berhasil diubah.");
                lastActivity.Text = "Changed " + (dgvConfig.Rows.Count - 1) + " " + att1 + "(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void UpdateXML(DataGridView dgvConfig, TextBox structureBox, TextBox xmlPath, ToolStripStatusLabel lastActivity, string att1,
            string att2, string att3)
        {
            try
            {
                for (int i = 0; i < dgvConfig.Rows.Count - 1; i++)
                {
                    //updateXML(, );
                    XmlNode node = xml.SelectSingleNode(structureBox.Text + "[@" + att1 + "='" + dgvConfig.Rows[i].Cells[0].Value.ToString() + "']");
                    node.Attributes[att2].Value = dgvConfig.Rows[i].Cells[1].Value.ToString();
                    node.Attributes[att3].Value = dgvConfig.Rows[i].Cells[2].Value.ToString();
                    xml.Save(xmlPath.Text);
                }
                MessageBox.Show(dgvConfig.Rows.Count - 1 + " " + att1 + " telah berhasil diubah.");
                lastActivity.Text = "Changed " + (dgvConfig.Rows.Count - 1) + " " + att1 + "(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void AddXML(TextBox structureBox, TextBox newKeyText, TextBox newValueText, TextBox xmlPath, 
            ToolStripStatusLabel lastActivity, TextBox keyBox, string att1, string att2)
        {
            try
            {
                string strucNode = structureBox.Text.Substring(0, structureBox.Text.LastIndexOf("/"));
                string newStrucNode = structureBox.Text.Substring(structureBox.Text.LastIndexOf("/") + 1);
                XmlNode node = xml.SelectSingleNode(strucNode);
                XmlNode newNode = xml.CreateElement(newStrucNode);

                XmlAttribute newKey = xml.CreateAttribute(att1);
                XmlAttribute newValue = xml.CreateAttribute(att2);

                newKey.Value = newKeyText.Text;
                newValue.Value = newValueText.Text;

                newNode.Attributes.Append(newKey);
                newNode.Attributes.Append(newValue);

                node.AppendChild(newNode);

                xml.Save(xmlPath.Text);
                MessageBox.Show("Telah berhasil ditambahkan " + att1 + " = " + keyBox.Text + " dengan " + att2 + " = " + newValueText.Text + ".");
                lastActivity.Text = "Added " + att1 + " (key : " + keyBox.Text + ", " + att2 + " : " + newValueText.Text + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }

        public void AddXML(TextBox structureBox, TextBox newKeyText, TextBox newValueText, TextBox newProvNameText, TextBox xmlPath,
            ToolStripStatusLabel lastActivity, TextBox keyBox, string att1, string att2, string att3)
        {
            try
            {
                string strucNode = structureBox.Text.Substring(0, structureBox.Text.LastIndexOf("/"));
                string newStrucNode = structureBox.Text.Substring(structureBox.Text.LastIndexOf("/") + 1);
                XmlNode node = xml.SelectSingleNode(strucNode);
                XmlNode newNode = xml.CreateElement(newStrucNode);

                XmlAttribute newName = xml.CreateAttribute(att1);
                XmlAttribute newConnString = xml.CreateAttribute(att2);
                XmlAttribute newProvName = xml.CreateAttribute(att3);

                newName.Value = newKeyText.Text;
                newConnString.Value = newValueText.Text;
                newProvName.Value = newProvNameText.Text;

                newNode.Attributes.Append(newName);
                newNode.Attributes.Append(newConnString);
                newNode.Attributes.Append(newProvName);

                node.AppendChild(newNode);

                xml.Save(xmlPath.Text);
                MessageBox.Show("Telah berhasil ditambahkan " + att1 + " = " + keyBox.Text + " dengan " + att2 + " = " + newValueText.Text + " dan " + att3 + " = " + newProvNameText.Text + ".");
                lastActivity.Text = "Added " + att1 + " (key : " + keyBox.Text + ", " + att2 + " : " + newValueText.Text + ", " + att3 + " : " + newProvNameText.Text + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex);
            }
        }
    }
}
