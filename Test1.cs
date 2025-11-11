using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lab5_Variant12; 

namespace Lab5_Tests
{
    [TestClass]
    public class ConcertTests
    {
        [TestMethod]
        public void Test_CountGenreWords_ReturnsCorrectValue()
        {
            // Arrange — створюємо тестовий концерт (вхідні дані)
            var concert = new Concert("Коваленко", "Рок Поп", new DateTime(2024, 5, 5), 1200);

            // Act — викликаємо метод, який тестуємо
            int wordCount = concert.CountGenreWords();

            // Assert — перевіряємо очікуваний результат
            Assert.AreEqual(5, wordCount, "Метод CountGenreWords працює некоректно!");
        }
    }
}
