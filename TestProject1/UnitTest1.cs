using DESComplementationPropertyDemonstration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using static DESComplementationPropertyDemonstration.Program;
using static DESComplementationPropertyDemonstration.Base64StringEnhance;

namespace TestProject1
{
    public class UnitTest1
    {
        static readonly byte[] key = new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        static readonly string plaintext = "Hello world!";

        [Fact]
        public void Test1()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            Main(new [] { k.ToString(), p.ToString() });

            var rb = File.ReadAllBytes("c.txt");
            var rrb = File.ReadAllBytes("cc.txt");

            var rbc = ComplementOf(rb);

            Assert.Equal(rbc, rrb);
        }

        [Fact]
        public void Test1_InMemory()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new byte[8];

            var rbc = ComplementOf(EncryptTextToMemory(p.ToString(), k.ToBytes(), iv));
            var rrb = EncryptTextToMemory(p.ToStringComplement(), k.ToBytesComplement(), iv);

            Assert.Equal(rbc, rrb);
        }

        [Fact]
        public void Test1ButValidateDecryption()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));
            var iv = new Base64StringEnhance(new byte[8]);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "m.txt", "mm.txt" });

            var rb = File.ReadAllBytes("m.txt");
            var rrb = File.ReadAllBytes("mm.txt");

            Assert.Equal(p.ToString(), DecryptTextFromMemory(rb, k.ToBytes(), iv.ToBytes()));
            Assert.Equal(p.ToStringComplement(), DecryptTextFromMemory(rrb, k.ToBytesComplement(), iv.ToBytes()));

            var rbc = ComplementOf(rb);

            Assert.Equal(rbc, rrb);
        }

        [Fact]
        public void Validation()
        {
            var DESalg = DES.Create();

            DESalg.Padding = PaddingMode.Zeros;

            EncryptTextToFile(plaintext, "test_validation.txt", DESalg.Key, DESalg.IV);

            Assert.NotEqual(plaintext, File.ReadAllText("test_validation.txt."));

            Assert.Equal(plaintext, DecryptTextFromFile("test_validation.txt", DESalg.Key, DESalg.IV));
        }

        [Fact]
        public void Validation2()
        {
            EncryptTextToFile(plaintext, "test_validation2.txt", key, new byte[8]);

            Assert.NotEqual(plaintext, File.ReadAllText("test_validation2.txt."));

            Assert.Equal(plaintext, DecryptTextFromFile("test_validation2.txt", key, new byte[8]));
        }

        [Fact]
        public void Validation3()
        {
            var k = new Base64StringEnhance(Convert.ToBase64String(key));

            var iv = new Base64StringEnhance(DES.Create().IV);

            EncryptTextToFile(plaintext, "test_validation3.txt", k.ToBytes(), iv.ToBytes());

            Assert.NotEqual(plaintext, File.ReadAllText("test_validation3.txt."));

            Assert.Equal(plaintext, DecryptTextFromFile("test_validation3.txt", k.ToBytes(), iv.ToBytes()));

            var r = new Base64StringEnhance(File.ReadAllBytes("test_validation3.txt"));
            Assert.Equal(plaintext, DecryptTextFromMemory(r.ToBytes(), k.ToBytes(), iv.ToBytes()));
        }

        [Fact]
        public void Validation4()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new Base64StringEnhance(DES.Create("DES").IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "b.txt", "bb.txt" });

            var r = new Base64StringEnhance(File.ReadAllBytes("b.txt"));
            var decryptedText = DecryptTextFromMemory(r.ToBytes(), k.ToBytes(), iv.ToBytes());
            Assert.Equal(p.ToString(), decryptedText);
        }

        [Fact]
        public void Validation5()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new Base64StringEnhance(DES.Create("DES").IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "a.txt", "aa.txt" });

            var decryptedText = DecryptTextFromFile("a.txt", k.ToBytes(), iv.ToBytes());
            Assert.Equal(p.ToString(), decryptedText);
        }

        [Fact]
        public void Validation6()
        {
            var k = new Base64StringEnhance(key);
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new Base64StringEnhance(DES.Create().IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "z.txt", "zz.txt" });

            var decryptedText = DecryptTextFromFile("z.txt", k.ToBytes(), iv.ToBytes());
            Assert.Equal(p.ToString(), decryptedText);

            decryptedText = DecryptTextFromFile("zz.txt", k.ToBytesComplement(), iv.ToBytes());
            Assert.Equal(p.ToStringComplement(), decryptedText);
        }

        [Theory]
        [InlineData(0b1000_0000, 0b0111_1111)]
        public void ValidationComplement(byte @in, byte @out)
        {
            Assert.Equal(new[] { @out }, ComplementOf(new[] { @in }));
        }

        [Fact]
        public void ValidationComplement2()
        {
            Assert.Equal(new byte [] { 0b0111_1111, 0b1000_0000 }, 
                ComplementOf(new byte [] { 0b1000_0000, 0b0111_1111 }));
        }

        [Fact]
        public void ValidationVector()
        {
            var desAlg = DES.Create();

            var iv = new Base64StringEnhance(desAlg.IV);

            Assert.Equal(iv.ToBytes(), desAlg.IV);
        }

        [Fact]
        public void FileVersusMemoryStream()
        {
            var des = DES.Create();

            EncryptTextToFile(plaintext, "test_fileVersusMemoryStream.txt", key, des.IV);
            var bytes = EncryptTextToMemory(plaintext, key, des.IV);

            var fileBytes = File.ReadAllBytes("test_fileVersusMemoryStream.txt");

            Assert.Equal(bytes, fileBytes);
        }

        [Fact]
        public void PlanTextAndbase64()
        {
            var strEnhanced = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            Assert.Equal(plaintext, strEnhanced.ToPlainText());
        }

        [Fact]
        public void BytesComplementIsSameLengthAsOriginalBytes()
        {
            var se = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            Assert.Equal(se.ToBytes().Length, se.ToBytesComplement().Length);
        }

        [Fact]
        public void StringComplementIsSameLengthAsOriginalString()
        {
            var se = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            Assert.Equal(se.ToString().Length, se.ToStringComplement().Length);
        }

        [Fact]
        public void ComplementOfAComplementIsTheSameAsOriginal()
        {
            var se = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var seC = se.ToBytesComplement();

            var seCe = new Base64StringEnhance(seC).ToBytesComplement();

            Assert.Equal(se.ToBytes(), seCe);
        }

        [Fact]
        public void ComplementOfAComplementIsTheSameAsOriginal2()
        {
            var se = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var seC = se.ToStringComplement();

            var seCe = new Base64StringEnhance(seC).ToStringComplement();

            Assert.Equal(se.ToString(), seCe);
        }

        [Fact]
        public void ValidateMainFunctionComplement()
        {
            var k = new Base64StringEnhance(ComplementOf(key));
            var p = new Base64StringEnhance(ComplementOf(Encoding.UTF8.GetBytes(plaintext)));

            var iv = new Base64StringEnhance(DES.Create().IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "d.txt", "dd.txt" });

            var decryptedText = DecryptTextFromFile("d.txt", k.ToBytes(), iv.ToBytes());
            Assert.Equal(p.ToString(), decryptedText);
            
            decryptedText = DecryptTextFromFile("dd.txt", k.ToBytesComplement(), iv.ToBytes());
            Assert.Equal(p.ToStringComplement(), decryptedText);

            var e = File.ReadAllBytes("d.txt");
            var ee = File.ReadAllBytes("dd.txt");

            Assert.Equal(e.Length, ee.Length);
        }

        [Fact]
        public void UsingKeyComplementToEncryptHelloWorld()
        {
            var k = new Base64StringEnhance(ComplementOf(key));
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new Base64StringEnhance(DES.Create("DES").IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "e.txt", "ee.txt" });

            var decryptedText = DecryptTextFromFile("e.txt", k.ToBytes(), iv.ToBytes());
            Assert.Equal(p.ToString(), decryptedText);

            var e = File.ReadAllBytes("e.txt");
            var ee = File.ReadAllBytes("ee.txt");

            Assert.Equal(e.Length, ee.Length);
        }

        [Fact]
        public void ValidateLengthWhenEncryptHelloWorld()
        {
            var k = new Base64StringEnhance(ComplementOf(key));
            var p = new Base64StringEnhance(Encoding.UTF8.GetBytes(plaintext));

            var iv = new Base64StringEnhance(DES.Create().IV);

            Main(new[] { k.ToString(), p.ToString(), iv.ToString(), "f.txt", "ff.txt" });

            var f = File.ReadAllBytes("f.txt");
            var ff = File.ReadAllBytes("ff.txt");

            Assert.Equal(f.Length, ff.Length);
        }
    }
}
