using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 文件压缩zip()、解压缩unzip()
    /// </summary>
    public class ZipTool
    {
        /// <summary>
        /// 根据给的文件参数，自动进行压缩或解压缩操作
        /// </summary>
        public static void Process(String[] files, String Password = null)
        {
            if (files.Length > 0)
            {
                if (files.Length == 1 && (files[0].ToLower().EndsWith(".zip") || files[0].ToLower().EndsWith(".rar")))
                {
                    unzip(files[0], null, Password, null);                  // 解压缩
                }
                else
                {
                    String zipPath = Tools.getPathNoExt(files[0]) + ".zip";	// 以待压缩的第一个文件命名生成的压缩文件
                    String BaseDir = Tools.getParent(files[0]);				// 获取第一个文件的父路径信息
                    if (files.Length == 1)									// 若载入的为单个目录，则已当前目录作为基础路径
                    {
                        String file = files[0];
                        if (Directory.Exists(file)) BaseDir = file + "\\";
                    }

                    String[] subFiles = Tools.getSubFiles(files);			// 获取args对应的所有目录下的文件列表
                    zip(zipPath, BaseDir, subFiles, Password, null);		// 对载入的文件进行压缩操作
                }
            }
        }

        /// <summary>
        /// 压缩所有文件files为zip
        /// </summary>
        public static bool zipFiles(String[] files, String Password = null, String[] ignoreNames = null)
        {
            return zip(null, null, files, Password, ignoreNames);
        }

        /// <summary>
        /// 压缩指定的文件或文件夹为zip
        /// </summary>
        public static bool zip(String file, String Password = null, String[] ignoreNames = null)
        {
            return zip(null, null, new String[] { file }, Password, ignoreNames);
        }

        /// <summary>
        /// 判断fileName中是否含有ignoreNames中的某一项
        /// </summary>
        private static bool ContainsIgnoreName(String fileName, String[] ignoreNames)
        {
            if (ignoreNames != null && ignoreNames.Length > 0)
            {
                foreach (string name in ignoreNames)
                {
                    if (fileName.Contains(name)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 压缩所有文件files，为压缩文件zipFile, 以相对于BaseDir的路径构建压缩文件子目录，ignoreNames指定要忽略的文件或目录
        /// </summary>
        public static bool zip(String zipPath, String BaseDir, String[] files, String Password = null, String[] ignoreNames = null)
        {
            if (files == null || files.Length == 0) return false;
            if (zipPath == null || zipPath.Equals("")) zipPath = Tools.getPathNoExt(files[0]) + ".zip";	// 默认以第一个文件命名压缩文件
            if (BaseDir == null || BaseDir.Equals("")) BaseDir = Tools.getParent(files[0]);				// 默认以第一个文件的父目录作为基础路径
            Console.WriteLine("所有待压缩文件根目录：" + BaseDir);

            try
            {
                Tools.mkdirs(Tools.getParent(zipPath));         // 创建目标路径
                Console.WriteLine("创建压缩文件：" + zipPath);

                FileStream input = null;
                ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipPath));
                if (Password != null && !Password.Equals("")) zipStream.Password = Password;

                files = Tools.getSubFiles(files);               // 获取子目录下所有文件信息
                for (int i = 0; i < files.Length; i++)
                {
                    if (ContainsIgnoreName(files[i], ignoreNames)) continue;    // 跳过忽略的文件或目录

                    String entryName = Tools.relativePath(BaseDir, files[i]);
                    zipStream.PutNextEntry(new ZipEntry(entryName));
                    Console.WriteLine("添加压缩文件：" + entryName);

                    if (File.Exists(files[i]))                  // 读取文件内容
                    {
                        input = File.OpenRead(files[i]);
                        Random rand = new Random();
                        byte[] buffer = new byte[10240];
                        int read = 0;
                        while ((read = input.Read(buffer, 0, 10240)) > 0)
                        {
                            zipStream.Write(buffer, 0, read);
                        }
                        input.Close();
                    }
                }
                zipStream.Close();
                Console.WriteLine("文件压缩完成！");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 解压文件 到指定的路径，可通过targeFileNames指定解压特定的文件
        /// </summary>
        public static bool unzip(String zipPath, String targetPath = null, String Password = null, String[] targeFileNames = null)
        {
            if (File.Exists(zipPath))
            {
                if (targetPath == null || targetPath.Equals("")) targetPath = Tools.getPathNoExt(zipPath);
                Console.WriteLine("解压文件：" + zipPath);
                Console.WriteLine("解压至目录：" + targetPath);

                try
                {
                    ZipInputStream zipStream = null;
                    FileStream bos = null;

                    zipStream = new ZipInputStream(File.OpenRead(zipPath));
                    if (Password != null && !Password.Equals("")) zipStream.Password = Password;

                    ZipEntry entry = null;
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (targeFileNames != null && targeFileNames.Length > 0)                // 若指定了目标解压文件
                        {
                            if (!ContainsIgnoreName(entry.Name, targeFileNames)) continue;      // 跳过非指定的文件
                        }

                        String target = targetPath + "\\" + entry.Name;
                        if (entry.IsDirectory) Tools.mkdirs(target); // 创建目标路径
                        if (entry.IsFile)
                        {
                            Tools.mkdirs(Tools.getParent(target));

                            bos = File.Create(target);
                            Console.WriteLine("解压生成文件：" + target);

                            int read = 0;
                            byte[] buffer = new byte[10240];
                            while ((read = zipStream.Read(buffer, 0, 10240)) > 0)
                            {
                                bos.Write(buffer, 0, read);
                            }
                            bos.Flush();
                            bos.Close();
                        }
                    }
                    zipStream.CloseEntry();

                    Console.WriteLine("解压完成！");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString()); ;
                }
            }
            return false;
        }

    }

}
