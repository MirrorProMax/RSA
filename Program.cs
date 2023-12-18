﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class Program
{
    public static void Main(string[] args)
    {
        string 根目录 = ".";

        //

        string 公有文件夹 = 根目录 + "/公有";//存放双方的公有内容
        string 私有文件夹_我方 = 根目录 + "/私有_我方";//保存我方的内容
        string 私有文件夹_对方_调试 = 根目录 + "/私有_对方_调试";//调试用的文件夹

        string[] 文件夹组 = { 公有文件夹, 私有文件夹_我方, 私有文件夹_对方_调试 };

        //

        string 我方公钥路径 = 公有文件夹 + "/我方公钥.xml";//公开,发给对方
        string 我方私钥路径 = 私有文件夹_我方 + "/我方私钥.xml";//私有,切勿泄漏

        string 对方公钥路径 = 公有文件夹 + "/对方公钥.xml";//公开,从对方获取
        string 对方私钥路径_调试 = 私有文件夹_对方_调试 + "/对方私钥_调试.xml";

        string 我方原文路径 = 私有文件夹_我方 + "/我方原文.txt";//私有,自己保留
        string 我方密文路径 = 公有文件夹 + "/我方密文.txt";//公开,可发给对方

        string 对方密文路径 = 公有文件夹 + "/对方密文.txt";//公开,从对方获取
        string 对方原文路径 = 私有文件夹_我方 + "/对方原文.txt";//私有,切勿泄漏

        string 对方原文路径_调试 = 私有文件夹_对方_调试 + "/对方原文_调试.txt";
        string 我方原文路径_调试 = 私有文件夹_对方_调试 + "/我方原文_调试.txt";

        //

        Console.WriteLine();
        Console.WriteLine($"RSA: 开始");

        确保文件夹存在(文件夹组);

        Console.WriteLine();
        Console.WriteLine($"请选择:");
        Console.WriteLine("1. 为我方创建新的'公钥'和'私钥'");
        Console.WriteLine("2. 通过'对方公钥'加密'我方原文'");
        Console.WriteLine("3. 通过'我方私钥'解密'对方密文'");
        Console.WriteLine("4. 调试: 为对方创建新的'公钥'和'私钥'");
        Console.WriteLine("5. 调试: 对方通过'我方公钥'加密'对方原文'");
        Console.WriteLine("6. 调试: 对方通过'对方私钥'解密'我方密文'");
        string? input = Console.ReadLine();
        int number;
        // 尝试将用户输入转换为数字  
        if (int.TryParse(input, out number) && number >= 1 && number <= 6)
        {
            // 根据数字调用不同的方法  
            switch (number)
            {
                case 1:
                    创建公钥和私钥(我方公钥路径, 我方私钥路径);
                    break;
                case 2:
                    加密文件(对方公钥路径, 我方原文路径, 我方密文路径);
                    break;
                case 3:
                    解密文件(我方私钥路径, 对方密文路径, 对方原文路径);
                    break;
                case 4:
                    创建公钥和私钥(对方公钥路径, 对方私钥路径_调试);
                    break;
                case 5:
                    加密文件(我方公钥路径, 对方原文路径_调试, 对方密文路径);
                    break;
                case 6:
                    解密文件(对方私钥路径_调试, 我方密文路径, 我方原文路径_调试);
                    break;
            }
        }
        else
        {
            Console.WriteLine("输入无效.");
        }

        Console.WriteLine();
        Console.WriteLine($"RSA: 结束");
    }


    public static void 创建公钥和私钥(string in公钥的路径, string in私钥的路径)
    {

        if (!控制台二次确认())
        {
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"RSA: 创建公钥和私钥 开始");

        using (var rsa = new RSACryptoServiceProvider(2048)) // 2048位密钥长度    
        {
            // 生成密钥对    
            rsa.PersistKeyInCsp = false; // 不要在内存中保存密钥    
            string 私钥 = rsa.ToXmlString(true); // 包含私钥的XML字符串
            string 公钥 = rsa.ToXmlString(false); // 不包含私钥的XML字符串

            // 将公钥写入公共密钥文件    
            File.WriteAllText(in公钥的路径, 公钥);

            // 将私钥写入私有密钥文件    
            File.WriteAllText(in私钥的路径, 私钥);
        }

        Console.WriteLine($"RSA: 创建公钥和私钥 完成");
        Console.WriteLine();
    }


    public static void 创建公钥和私钥_批量(string[] in公钥的路径组, string[] in私钥的路径组)
    {
        if (!控制台二次确认())
        {
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"RSA: 创建公钥和私钥 开始");

        using (var rsa = new RSACryptoServiceProvider(2048)) // 2048位密钥长度    
        {
            // 生成密钥对    
            rsa.PersistKeyInCsp = false; // 不要在内存中保存密钥    
            string 私钥 = rsa.ToXmlString(true); // 包含私钥的XML字符串
            string 公钥 = rsa.ToXmlString(false); // 不包含私钥的XML字符串

            // 将公钥写入公共密钥文件    
            // File.WriteAllText(in公钥的路径, 公钥);
            写入文本_多路径(in公钥的路径组, 公钥);

            // 将私钥写入私有密钥文件    
            // File.WriteAllText(in私钥的路径组, 私钥);
            写入文本_多路径(in私钥的路径组, 私钥);
        }

        Console.WriteLine($"RSA: 创建公钥和私钥 完成");
        Console.WriteLine();
    }


    public static void 加密文件(string in公钥的路径, string in原文的路径, string out文件的路径)
    {
        Console.WriteLine();
        Console.WriteLine($"RSA: 加密 开始");
        using (var rsa = new RSACryptoServiceProvider(2048)) // 2048位密钥长度    
        {
            // 读取输入文件内容    
            string 原文 = File.ReadAllText(in原文的路径);
            Console.WriteLine($"原文: {原文}");

            //读取公钥
            string 公钥 = File.ReadAllText(in公钥的路径);

            // 从XML文件中加载公钥    
            rsa.FromXmlString(公钥);

            // 使用公钥加密文件内容    
            byte[] 加密后的内容_byte = rsa.Encrypt(Encoding.UTF8.GetBytes(原文), false);

            // 将加密后的数据转换为字符串    
            string 加密后的内容_字符串 = Convert.ToBase64String(加密后的内容_byte);
            Console.WriteLine($"密文: {加密后的内容_字符串}");

            // 将加密后的字符串写入输出文件    
            File.WriteAllText(out文件的路径, 加密后的内容_字符串);
        }
        Console.WriteLine($"RSA: 加密 完成");
        Console.WriteLine();
    }

    public static void 解密文件(string in私钥的路径, string in文件的路径, string out文件的路径)
    {
        Console.WriteLine();
        Console.WriteLine($"RSA: 解密 开始");
        using (var rsa = new RSACryptoServiceProvider(2048)) // 2048位密钥长度  
        {
            // 读取输入文件内容  
            string 密文 = File.ReadAllText(in文件的路径);
            Console.WriteLine($"密文: {密文}");

            //读取私钥
            string 私钥 = File.ReadAllText(in私钥的路径);

            // 从XML文件中加载私钥  
            rsa.FromXmlString(私钥);

            // 将加密后的字符串转换为字节数组  
            byte[] encryptedData = Convert.FromBase64String(密文);

            // 使用私钥解密文件内容  
            byte[] decryptedData = rsa.Decrypt(encryptedData, false);

            // 将解密后的数据转换为字符串  
            string 原文 = Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine($"原文: {原文}");

            // 将解密后的内容写入输出文件  
            File.WriteAllText(out文件的路径, 原文);
        }
        Console.WriteLine($"RSA: 解密 完成");
        Console.WriteLine();
    }


    public static void 确保文件夹存在(string[] inStringArray)
    {

        // 循环检查每个文件夹是否已存在,如果不存在则创建  
        foreach (string folder in inStringArray)
        {
            // 如果文件夹不存在,则创建它  
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Console.WriteLine($"创建了文件夹: {folder}");
            }
            else
            {
                Console.WriteLine($"文件夹已存在: {folder}");
            }
        }
    }


    public static void 写入文本_多路径(string[] in路径组, string in内容)
    {
        foreach (string 路径 in in路径组)
        {
            File.WriteAllText(路径, in内容);
        }
    }


    public static bool 控制台二次确认()
    {
        // 使用方法
        //⬇️⬇️⬇️
        // if (!控制台二次确认())
        // {
        //     return;
        // }
        //⬆️⬆️⬆️


        Console.Write("二次确认请输入'y':");
        char userInput = Convert.ToChar(Console.ReadLine().ToLower());
        if (userInput == 'y')
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}