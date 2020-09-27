using System;
using System.Collections.Generic;
using System.Drawing;
using Xunit;
namespace XUnitTestSobel
{//“ест не проходит, не хочет видеть подключенную EMGU.CV, € пыталс€, не успеваю уже победить, извините :/
    public class UnitTest1
    {
        [Fact]
        public void InputImageNotNull()
        {
            // Arrange
            Sobel.LogicHandler handler = new Sobel.LogicHandler();
            // Act
            handler.CutImage();
            // Assert
            Assert.NotEmpty(handler.Images);
        }
    }
}
