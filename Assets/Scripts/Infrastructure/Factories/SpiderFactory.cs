using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Behaviours;
using Core.Binders;
using Core.Models;
using Core.Models.Components;
using Infrastructure.Services.Input;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories
{
    public class SpiderFactory
    {
        private readonly ViewFactory _viewFactory;
        private FixedTickService _fixedTickService;
        private IInputService _inputService;
        private DamageReceiverService _damageReceiverService;
        private ServiceComponentFactory _serviceComponentFactory;
        private SpiderService _spiderService;

        public SpiderFactory(ViewFactory viewFactory, FixedTickService fixedTickService, 
            DamageReceiverService damageReceiverService, ServiceComponentFactory serviceComponentFactory, 
            SpiderService spiderService, DamageReceiverService damagerReceiverService)
        {
            _spiderService = spiderService;
            _damageReceiverService = damagerReceiverService;
            _serviceComponentFactory = serviceComponentFactory;
            _damageReceiverService = damageReceiverService;
            _fixedTickService = fixedTickService;
            _viewFactory = viewFactory;
        }

        public void CreatePlayer(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider();
            behaviour.Transform.position = position;
            
            var stats = new SpiderStats(100, 10);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider);
            var model = new Spider(stats, components);
            var binder = new Binder();
            
            BindPlayer(model, binder);
            BindHealth(model, binder, behaviour);
        }
        
        public void CreateEnemy(Vector3 position)
        {
            var behaviour = _viewFactory.DefaultSpider();
            behaviour.Transform.position = position;
            
            var stats = new SpiderStats(50, 5);
            var components = new SpiderComponents(behaviour.Transform, behaviour.Collider);
            var model = new Spider(stats, components);
            var binder = new Binder();
            
            BindEnemy(model, binder);
            BindHealth(model, binder, behaviour);
        }

        private void BindPlayer(Spider model, Binder binder)
        {
            binder.LinkEvent(_inputService.Attacked, _serviceComponentFactory.PlayerDamager(model, 1).TryDamage);
            
            binder.LinkHolder(_fixedTickService.FixedTickableHolder, _serviceComponentFactory.PlayerMovement(model));
            binder.LinkHolder(_damageReceiverService.DamageReceiverHolder, _serviceComponentFactory.DamageReceiver(model));
            binder.LinkHolder(_spiderService.SpiderHolder, model);
        }
        
        private void BindEnemy(Spider model, Binder binder)
        {
            var damager = _serviceComponentFactory.Damager(model, 1);
            var damageReceiver = _serviceComponentFactory.DamageReceiver(model);
            var enemyMovement = _serviceComponentFactory.EnemyMovement(model);
            
            binder.LinkHolder(_fixedTickService.FixedTickableHolder, enemyMovement);
            binder.LinkHolder(_damageReceiverService.DamageReceiverHolder, damageReceiver);
            binder.LinkHolder(_fixedTickService.TickableHolder, damager);
            binder.LinkHolder(_spiderService.SpiderHolder, model);
        }

        private void BindHealth(Spider model, Binder binder, SpiderBehaviour behaviour)
        {
            binder.Bind(model.Stats.Health, (health => behaviour.HealthText.text = $"hp: {health}"));
            
            binder.Bind(model.Stats.Health, (health => {
                if (health <= 0)
                {
                    binder.Dispose();
                }
            }));
        }
    }

    public class ServiceComponentFactory
    {
        private IInputService _inputService;
        private SpiderService _spiderService;
        private CollisionService _collisionService;
        private DamageReceiverService _damageReceiverService;

        public ServiceComponentFactory(IInputService inputService, SpiderService spiderService, CollisionService collisionService, DamageReceiverService damageReceiverService)
        {
            _spiderService = spiderService;
            _collisionService = collisionService;
            _damageReceiverService = damageReceiverService;
            _inputService = inputService;
        }

        public PlayerMovement PlayerMovement(Spider model)
        {
            var movement = new PlayerMovement(_inputService, model);
            return movement;
        }

        public IDamageReceiver DamageReceiver(Spider model)
        {
            var damaging = new DamageReceiver(model);
            return damaging;
        }

        public IFixedTickable EnemyMovement(Spider model)
        {
            var movement = new EnemyMovement(model, _spiderService);
            return movement;
        }

        public EnemyDamager Damager(Spider model, float cooldown)
        {
            var damager = new EnemyDamager(model, cooldown, _damageReceiverService, _collisionService);
            return damager;
        }

        public PlayerDamager PlayerDamager(Spider model, int cooldown)
        {
            var damager = new PlayerDamager(model, cooldown, _damageReceiverService, _collisionService);
            return damager;
        }
    }

    public class DamageReceiver : IDamageReceiver
    {
        private Spider _model;

        public SpiderTag Tag => _model.Stats.Tag;

        public DamageReceiver(Spider model)
        {
            _model = model;
        }

        public void GetDamage(int value)
        {
            _model.Stats.Health.Decrease(value);
        }
    }

    public class PlayerMovement : IFixedTickable
    {
        private IInputService _inputService;
        private Spider _model;

        public PlayerMovement(IInputService inputService, Spider model)
        {   
            _model = model;
            _inputService = inputService;
        }

        public void FixedTick()
        {
            _model.Components.Transform.Translate(_inputService.Moving());
        }
    }
    
    public class PlayerDamager
    {
        private Spider _model;
        private DamageReceiverService _damageReceiverService;
        private float _attackCooldown;
        private float _time;
        private CollisionService _collisionService;

        public PlayerDamager(Spider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, CollisionService collisionService)
        {
            _collisionService = collisionService;
            _attackCooldown = attackCooldown;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (_time < Time.time)
                return;
            
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Player);
            var filterCollision = new Filter<Spider>((spider) =>
                _collisionService.HasCollision(_model.Components.Collider, spider.Components.Collider));
            if (_damageReceiverService.TryPerform(_model.Stats.Health.Value, filterTag, filterCollision))
            {
                _time = Time.time+_attackCooldown;
            }
        }
    }
    
    public class EnemyMovement : IFixedTickable
    {
        private Spider _model;
        private SpiderService _spiderHolder;

        public EnemyMovement(Spider model, SpiderService spiderHolder)
        {
            _spiderHolder = spiderHolder;
            _model = model;
        }

        public void FixedTick()
        {
            if (_spiderHolder.TryFind(SpiderTag.Player, out Spider player))
            {
                var delta = (player.Components.Transform.position - _model.Components.Transform.position).normalized;
                _model.Components.Transform.Translate(delta);
            }
        }
    }
    
    public class EnemyDamager : IDamager, ITickable
    {
        private Spider _model;
        private DamageReceiverService _damageReceiverService;
        private CollisionService _collisionService;
        private float _attackCooldown;
        private float _time;

        public EnemyDamager(Spider model, float attackCooldown, 
            DamageReceiverService damageReceiverService, CollisionService collisionService)
        {
            _attackCooldown = attackCooldown;
            _collisionService = collisionService;
            _damageReceiverService = damageReceiverService;
            _model = model;
        }

        public void TryDamage()
        {
            if (_time < Time.time)
                return;
            
            var filterTag = new Filter<Spider>((spider) => spider.Stats.Tag == SpiderTag.Player);
            var filterCollision = new Filter<Spider>((spider) =>
                _collisionService.HasCollision(_model.Components.Collider, spider.Components.Collider));
            
            if (_damageReceiverService.TryPerform(_model.Stats.Health.Value, filterTag, filterCollision))
            {
                _time = Time.time+_attackCooldown;
            }
        }

        public void Tick()
        {
            TryDamage();
        }
    }

    public class CollisionService
    {
        public bool HasCollision(Collider2D first, Collider2D second)
        {
            return first.IsTouching(second);
        }
    }

    public interface IDamager
    {
        public void TryDamage();
    }

    public class Filter<T> :IFilter where T : class
    {
        private Func<T, bool> _condition;

        public Filter(Func<T, bool> condition)
        {
            _condition = condition;
        }

        public bool IsMet<TItem>(TItem item)
        {
            if(item is T typedItem)
                return _condition.Invoke(typedItem);
            return false;
        }
    }

    public interface IFilter
    {
        bool IsMet<TItem>(TItem item);
    }

    public class SpiderService
    {
        public ItemHolder<Spider> SpiderHolder = new();

        public bool TryFind(SpiderTag tag, out Spider spider)
        {
            var spiders = SpiderHolder.Get();
            var found = spiders.FirstOrDefault((spider1 => spider1.Stats.Tag == tag));
            if (found != default)
            {
                spider = found;
                return true;
            }

            spider = null;
            return false;
        }
    }

    public enum SpiderTag
    {
        Player,
        Enemy,
    }

    public class FixedTickService : IFixedTickable, ITickable, IDisposable
    {
        public ItemHolder<IFixedTickable> FixedTickableHolder = new();
        public ItemHolder<ITickable> TickableHolder = new();

        public void FixedTick()
        {
            var items = FixedTickableHolder.Get();
            foreach (var item in items)
            {
                item.FixedTick();
            }
        }

        public void Tick()
        {
            var items = TickableHolder.Get();
            foreach (var item in items)
            {
                item.Tick();
            }
        }

        public void Dispose()
        {
            FixedTickableHolder.Dispose();
            TickableHolder.Dispose();
        }
    }

    public class DamageReceiverService : IDisposable
    {
        public readonly ItemHolder<IDamageReceiver> DamageReceiverHolder = new();

        public bool TryPerform(int value, params IFilter[] filters)
        {
            var receivers = DamageReceiverHolder.Get();
            var gotDamage = false;
            foreach (var receiver in receivers)
            {
                if (IsMet(receiver, filters))
                {
                    receiver.GetDamage(value);
                    gotDamage = true;
                }
            }

            return gotDamage;
        }

        private bool IsMet(IDamageReceiver receiver, IFilter[] filters)
        {
            foreach (var filter in filters)
            {
                if (filter.IsMet(receiver) == false)
                    return false;
            }

            return true;
        }

        public void Dispose()
        {
            DamageReceiverHolder.Dispose();
        }
    }

    public interface IDamageReceiver
    {
        void GetDamage(int value);
    }

    public class ItemHolder<T> : IDisposable
    {
        private List<T> _items = new();

        public T[] Get()
        {
            _items.RemoveAll(item => item == null);
            return _items.ToArray();
        }

        public void Add(T item)
        {
            if (_items.Contains(item))
                return;
            _items.Add(item);
        }

        public void Remove(T item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        public void Dispose()
        {
            _items.Clear();
        }
    }
}