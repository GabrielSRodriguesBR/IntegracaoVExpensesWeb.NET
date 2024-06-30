using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CriptoPass
{
    public static class CriptoPass
    {

        private static readonly byte[] chave = Convert.FromBase64String("ocR8lQYkv4v9lRZ+HlfO2pNkFleDx4t2RG2+rzX8MxI="); // Chave de 256 bits (32 bytes)
        private static readonly byte[] vetorInicializacao = Convert.FromBase64String("2ZUZvYRakTXfBa9qU0o6pA=="); // IV de 128 bits (16 bytes)

        public static string CriptografarSenha(string pass)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = chave;
                aesAlg.IV = vetorInicializacao;

                // Cria um encryptor para executar a transformação da stream
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Stream para armazenar o texto encriptado
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // CryptoStream para realizar a encriptação
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // StreamWriter para escrever os dados encriptados no CryptoStream
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Escreve os dados encriptados
                            swEncrypt.Write(pass);
                        }
                    }

                    // Retorna o texto encriptado como uma string Base64
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DescriptografarSenha(string textoEncriptado)
        {
            byte[] textoEncriptadoBytes = Convert.FromBase64String(textoEncriptado);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = chave;
                aesAlg.IV = vetorInicializacao;

                // Cria um decryptor para executar a transformação da stream
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Stream para armazenar o texto desencriptado
                using (MemoryStream msDecrypt = new MemoryStream(textoEncriptadoBytes))
                {
                    // CryptoStream para realizar a desencriptação
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // StreamReader para ler os dados desencriptados do CryptoStream
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Retorna os dados desencriptados como uma string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }



    }
}
