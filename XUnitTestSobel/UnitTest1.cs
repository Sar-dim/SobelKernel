using System;
using System.Collections.Generic;
using System.Drawing;
using Xunit;
namespace XUnitTestSobel
{//���� �� ��������, �� ����� ������ ������������ EMGU.CV, � �������, �� ������� ��� ��������, �������� :/
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
