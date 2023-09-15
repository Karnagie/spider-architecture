using System.Collections.Generic;
using Core.Models.Services;
using Infrastructure.Services.System;
using Microsoft.Win32;
using Moq;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Core.Models.Systems.Tests
{
    public class SpiderLegServiceTests
    {
        [Test]
        public void IsConnecting_NoLinkers_ReturnsFalse()
        {
            // Arrange
            var systemServiceMock = new SystemService();
            var spiderLegService = new SpiderLegService(systemServiceMock);
            var mockSpider = new Mock<ISpider>();

            // Act
            var result = spiderLegService.IsConnecting(mockSpider.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsConnecting_NoLegsConnecting_ReturnsFalse()
        {
            // Arrange
            var systemServiceMock = new SystemService();
            var spiderLegService = new SpiderLegService(systemServiceMock);
            var linkerMock = new SystemLinker();
            var mockSpider = new Mock<ISpider>();

            systemServiceMock.Add(linkerMock);
            linkerMock.Add(mockSpider.Object);

            // Act
            var result = spiderLegService.IsConnecting(mockSpider.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsConnecting_LegConnecting_ReturnsTrue()
        {
            // Arrange
            var systemServiceMock = new SystemService();
            var linkerMock = new SystemLinker();
            
            var spiderLegService = new SpiderLegService(systemServiceMock);
            
            var leg = new Mock<ILegSystem>();
            leg.Setup(leg => leg.Connecting()).Returns(true);
            
            var mockSpider = new Mock<ISpider>();

            linkerMock.Add(leg.Object);
            linkerMock.Add(mockSpider.Object);

            systemServiceMock.Add(linkerMock);


            // Act
            var result = spiderLegService.IsConnecting(mockSpider.Object);
        
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConnecting_NotAllLegsConnecting_ReturnsTrue()
        {
            // Arrange
            var systemServiceMock = new SystemService();
            var linkerMock = new SystemLinker();
            
            var spiderLegService = new SpiderLegService(systemServiceMock);
            
            var leg = new Mock<ILegSystem>();
            leg.Setup(legSystem => legSystem.Connecting()).Returns(false);
            var leg1 = new Mock<ILegSystem>();
            leg1.Setup(legSystem => legSystem.Connecting()).Returns(false);
            var leg2 = new Mock<ILegSystem>();
            leg2.Setup(legSystem => legSystem.Connecting()).Returns(true);
            
            var mockSpider = new Mock<ISpider>();

            linkerMock.Add(leg.Object);
            linkerMock.Add(leg1.Object);
            linkerMock.Add(leg2.Object);
            linkerMock.Add(mockSpider.Object);

            systemServiceMock.Add(linkerMock);


            // Act
            var result = spiderLegService.IsConnecting(mockSpider.Object);
        
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConnecting_LegsNotConnectedToSpider_ReturnsFalse()
        {
            // Arrange
            var systemServiceMock = new SystemService();
            var linkerLegs = new SystemLinker();
            var linkerSpider = new SystemLinker();
            
            var spiderLegService = new SpiderLegService(systemServiceMock);
            
            var leg = new Mock<ILegSystem>();
            leg.Setup(legSystem => legSystem.Connecting()).Returns(false);
            var leg1 = new Mock<ILegSystem>();
            leg1.Setup(legSystem => legSystem.Connecting()).Returns(false);
            var leg2 = new Mock<ILegSystem>();
            leg2.Setup(legSystem => legSystem.Connecting()).Returns(true);
            
            var mockSpider = new Mock<ISpider>();

            linkerLegs.Add(leg.Object);
            linkerLegs.Add(leg1.Object);
            linkerLegs.Add(leg2.Object);
            linkerSpider.Add(mockSpider.Object);

            systemServiceMock.Add(linkerLegs);
            systemServiceMock.Add(linkerSpider);

            // Act
            var result = spiderLegService.IsConnecting(mockSpider.Object);
        
            // Assert
            Assert.IsFalse(result);
        }
    }
}