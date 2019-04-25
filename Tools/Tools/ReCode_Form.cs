using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace Tools
{
    public partial class ReCode_Form : Form
    {
        public ReCode_Form()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 3;
        }

        String[] files = new String[] { };
        Encoding encode = Encoding.Default;


        private void textBox_DragEnter(object sender, DragEventArgs e)
        {
            DragDropTool.Form_DragEnter(sender, e);
        }

        private void textBox_DragDrop(object sender, DragEventArgs e)
        {
            files = DragDropTool.Form_DragDrop(sender, e);
        }

        /// <summary>
        /// 重编码载入的所有文件
        /// </summary>
        private void button_Click(object sender, EventArgs e)
        {
            if (checkBox.Checked) RenameTool.ReName(files);
            RecodeTool.ReCode(files, encode);
            MessageBox.Show("重编码完成");
        }

        /// <summary>
        /// 设置编码格式
        /// </summary>
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox.Text.Equals("ANSI")) encode = Encoding.Default;
            else if(comboBox.Text.Equals("Unicode")) encode = Encoding.Unicode;
            else if(comboBox.Text.Equals("Unicode big endian")) encode = Encoding.BigEndianUnicode;
            else if(comboBox.Text.Equals("UTF-8")) encode = Encoding.UTF8;
        }

    }
}
