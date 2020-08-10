using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Generic;


namespace Excel2Json
{
    /// <summary>
    /// ���ݱ�ͷ������C#�ඨ�����ݽṹ
    /// ��ͷʹ�����ж��壺�ֶ����ơ��ֶ����͡�ע��
    /// </summary>
    class ExportCSharp
    {
        /// <summary>
        /// �ֶ�
        /// </summary>
        struct FieldInfo
        {
            /// <summary>
            /// ������
            /// </summary>
            public string name;
            /// <summary>
            /// ����
            /// </summary>
            public string type;
            /// <summary>
            /// ע��
            /// </summary>
            public string notes;
        }

        DataTable mSheet;
        List<FieldInfo> mFieldInfoList;

        public ExportCSharp(DataTable sheet)
        {
            mSheet = sheet;
        }

        //����ΪCSharp
        public bool Export()
        {
            //First Row as Column Name
            if (mSheet.Rows.Count < 2)
                return false;

            mFieldInfoList = new List<FieldInfo>();
            DataRow typeRow = mSheet.Rows[0];
            DataRow notesRow = mSheet.Rows[1];

            for (int i = 0; i < mSheet.Columns.Count; i++)
            {
                var column = mSheet.Columns[i];
                if (column.ToString().Length <= 0 || typeRow[column].ToString().Length <= 0) continue;
                if (column.ToString().Length >= 6)
                {
                    string co = column.ToString().Substring(0, 6);
                    if (co == "Column")
                    {
                        continue;
                    }
                }

                FieldInfo field;
                field.name = column.ToString();
                field.type = typeRow[column].ToString();
                field.notes = notesRow[column].ToString();
                mFieldInfoList.Add(field);
            }

            return true;
        }

        public void SaveToFile(string strTemplatePath, string filePath, Encoding encoding)
        {
            // ����Excel�ļ�
            using (FileStream templateFile = File.Open(strTemplatePath, FileMode.Open, FileAccess.Read))
            {
                if (mFieldInfoList == null)
                    throw new Exception("FieldInfo�ṹ������Ϊ�գ��ֶ�Ϊ�գ���");

                byte[] templateByte = new byte[templateFile.Length];
                templateFile.Read(templateByte, 0, templateByte.Length);
                string strTemplate = Encoding.UTF8.GetString(templateByte);

                string tableName = Path.GetFileNameWithoutExtension(filePath);

                // ����ֶι���
                StringBuilder sbClassField = new StringBuilder();
                for (int i = 0; i < mFieldInfoList.Count; ++i)
                {
                    var field = mFieldInfoList[i];

                    if (i == 0)
                        sbClassField.AppendFormat("/// <summary> \n \t\t/// {2}\n \t\t/// </summary> \n \t\tpublic {0} {1};", field.type, field.name, field.notes);
                    else
                        sbClassField.AppendFormat("\t\t/// <summary> \n \t\t/// {2}\n \t\t/// </summary> \n \t\tpublic {0} {1};", field.type, field.name, field.notes);

                    if (i != mFieldInfoList.Count - 1)
                        sbClassField.AppendLine();
                }

                var a = sbClassField.ToString();

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(strTemplate, tableName, a);

                //�����ļ�
                string newFilePath = filePath.Substring(0, filePath.Length - 3) + "Cfg.cs";
                using (FileStream file = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter writer = new StreamWriter(file, encoding))
                        writer.Write(sb.ToString());
                }
            }
        }
    }
}