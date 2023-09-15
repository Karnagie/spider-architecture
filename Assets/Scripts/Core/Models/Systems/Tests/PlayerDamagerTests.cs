using Core.Models.Components;
using Core.Models.Services;
using Infrastructure.Helpers;
using Infrastructure.Services.System;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Core.Models.Systems.Tests
{
    public class PlayerDamagerTests
    {
        [Test]
        public void TryDamage_Damage30_Hp70()
        {
            // Arrange. 
            var systemService = new SystemService();
            var damageReceiverService = new DamageReceiverService(systemService);
            
            var physicsService = new Mock<IPhysicsService>();
            physicsService.Setup((service => service.HasCollision(It.IsAny<Collider2D>(), It.IsAny<Collider2D>())))
                .Returns(true);

            var stats = new SpiderStats(110, 30, SpiderTag.Player);
            var go = new GameObject();
            var components = new SpiderComponents(go.transform, new Collider2D(), new Rigidbody2D());
            var spiderDamagerMock = new Mock<ISpider>();
            spiderDamagerMock.Setup((sp => sp.Stats)).Returns(stats);
            spiderDamagerMock.Setup((sp => sp.Components)).Returns(components);
            var damager = new PlayerDamager(spiderDamagerMock.Object, 0, damageReceiverService,
                physicsService.Object);
            
            var enemyStats = new SpiderStats(100, 20, SpiderTag.Enemy);
            var enemyComponents = new SpiderComponents(go.transform, new Collider2D(), new Rigidbody2D());
            var enemyMock = new Mock<ISpider>();
            enemyMock.Setup((sp => sp.Stats)).Returns(enemyStats);
            enemyMock.Setup((sp => sp.Components)).Returns(enemyComponents);
            var enemyLinker = new SystemLinker();
            enemyLinker.Add(enemyMock.Object);
            enemyLinker.Add(new DamageReceiver(enemyMock.Object));
            
            systemService.Add(enemyLinker);

            // Act.
            damager.TryDamage();

            // Assert.
            Assert.AreEqual(70, enemyMock.Object.Stats.Health.Value);
        }
    }
}