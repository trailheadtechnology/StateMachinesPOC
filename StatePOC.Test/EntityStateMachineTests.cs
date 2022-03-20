using NUnit.Framework;
using System;

namespace StatePOC.Test
{
    public class EntityStateMachineTests
    {
        [Test]
        public void CanCreateTest()
        {
            // Arrange // Act
            var entityStateMachine = new EntityStateMachine();

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.NotNull(entityStateMachine);
        }
        
        [Test]
        public void CanStartLoadTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();

            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.StateMachine.IsInState(EntityStateMachine.EntityState.Loading));
        }


        [Test]
        public void CanCompleteLoadTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Setted);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.StateMachine.IsInState(EntityStateMachine.EntityState.Set));
        }


        [Test]
        public void LoadCantReentryTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Act
            Assert.Throws<InvalidOperationException>(() => {
                entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);
            });
        }


        [Test]
        public void FirstLoadFromServerTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            
            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.EntitySourceValue,Is.EqualTo(EntityStateMachine.EntitySource.Server));
        }

        [Test]
        public void SecondLoadFromLocalTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.LoadFail);

            Console.WriteLine(entityStateMachine.StateMachine);
            Console.WriteLine(entityStateMachine.EntitySourceValue);

            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.EntitySourceValue,Is.EqualTo(EntityStateMachine.EntitySource.Local));
        }

        [Test]
        public void ThirdLoadNewTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.LoadFail);
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.LoadFail);

            Console.WriteLine(entityStateMachine.StateMachine);

            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.EntitySourceValue, Is.EqualTo(EntityStateMachine.EntitySource.New));
        }

        [Test]
        public void CanCloseTest()
        {
            // Arrange
            var entityStateMachine = new EntityStateMachine();
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Load);
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Setted);
            Console.WriteLine(entityStateMachine.StateMachine);

            // Act
            entityStateMachine.StateMachine.Fire(EntityStateMachine.EntityTrigger.Close);

            // Assert
            Console.WriteLine(entityStateMachine.StateMachine);
            Assert.That(entityStateMachine.StateMachine.IsInState(EntityStateMachine.EntityState.NotSet));
            Assert.That(entityStateMachine.EntitySourceValue, Is.EqualTo(EntityStateMachine.EntitySource.NotSet));
        }

    }
}