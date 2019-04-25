using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 通用功能函数
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// 检测目录是否存在，若不存在则创建
        /// </summary>
        public static void mkdirs(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 获取去除拓展名的文件路径
        /// </summary>
        public static String getPathNoExt(String path)
        {
            if (File.Exists(path)) return Directory.GetParent(path).FullName + "\\" + Path.GetFileNameWithoutExtension(path);
            else return Directory.GetParent(path).FullName + "\\" + Path.GetFileName(path);
        }

        /// <summary>
        /// 获取父目录的路径信息
        /// </summary>
        public static String getParent(String path)
        {
            return System.IO.Directory.GetParent(path).FullName + "\\";
        }


        /// <summary>
        /// 获取父目录的路径信息
        /// </summary>
        public static String getFileName(String path)
        {
            return System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// 获取filePath的相对于BaseDir的路径
        /// </summary>
        public static String relativePath(String BaseDir, String filePath)
        {
            String relativePath = "";
            if (filePath.StartsWith(BaseDir)) relativePath = filePath.Substring(BaseDir.Length);
            return relativePath;
        }


        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 获取paths路径下所有文件信息
        /// </summary>
        public static String[] getSubFiles(String[] Paths)
        {
            List<String> list = new List<String>();	        // paths路径下所有文件信息

            foreach (String path in Paths)
            {
                List<String> subFiles = getSubFiles(path);	// 获取路径path下所有文件列表信息
                list = ListAdd(list, subFiles);
            }

            String[] A = List2Array(list);					// 转化为数组形式

            return A;
        }

        /// <summary>
        /// 合并list1和list2到新的list
        /// </summary>
        public static List<String> ListAdd(List<String> list1, List<String> list2)
        {
            List<String> list = new List<String>();

            foreach (String path in list1) if (!list.Contains(path)) list.Add(path);
            foreach (String path in list2) if (!list.Contains(path)) list.Add(path);

            return list;
        }

        /// <summary>
        /// 获取file目录下所有文件列表
        /// </summary>
        public static List<String> getSubFiles(String file)
        {
            List<String> list = new List<String>();

            if (File.Exists(file))
            {
                if (!list.Contains(file)) list.Add(file);
            }

            if (Directory.Exists(file))
            {
                // 获取目录下的文件信息
                foreach (String iteam in Directory.GetFiles(file))
                {
                    if (!list.Contains(iteam)) list.Add(iteam);
                }

                // 获取目录下的子目录信息
                foreach (String iteam in Directory.GetDirectories(file))
                {
                    List<String> L = getSubFiles(iteam);	// 获取子目录下所有文件列表
                    foreach (String path in L)
                    {
                        if (!list.Contains(path)) list.Add(path);
                    }
                }

                // 记录当前目录
                if (Directory.GetFiles(file).Length == 0 && Directory.GetDirectories(file).Length == 0)
                {
                    if (!list.Contains(file)) list.Add(file + "\\");
                }
            }

            return list;
        }

        /// <summary>
        /// 转化list为数组
        /// </summary>
        public static String[] List2Array(List<String> list)
        {
            int size = (list == null ? 0 : list.Count);
            String[] A = new String[size];

            int i = 0;
            foreach (String S in list)
            {
                A[i++] = S;
            }

            return A;
        }

    }

}
