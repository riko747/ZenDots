using System.Collections.Generic;
using System.Linq;
using Entities.Dot;
using Interfaces.Managers;
using Interfaces.Strategies;
using Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawners
{
    public class DefaultModeDotSpawner : ISpawnStrategy
    {
        [Inject] private IResourcesManager _resourcesManager;
        [Inject] private IGameManager _gameManager;
        [Inject] private ILevelManager _levelManager;
        [Inject] private IDoTweenManager _doTweenManager;
        
        private Vector3[] _gameAreaCorners;
        private float _defaultDotsSizeInPixels;
        private RectTransform _gameArea;

        private const int MaxChecks = 100;
        
        private int _dotsCount;

        public DefaultModeDotSpawner(RectTransform gameArea)
        {
            _gameArea = gameArea;
        }
        
        public void Spawn()
        {
            InitializeFields();

            var numbers = GenerateUniqueNumbers(_dotsCount);
            SpawnDots(numbers);
        }

        private void InitializeFields()
        {
            var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            _defaultDotsSizeInPixels = dotPrefab.GetDotSizeInPixelsX();
            _gameAreaCorners = new Vector3[4];
            _dotsCount = _levelManager.GetCurrentLevel().dotCount;
        }

        private List<int> GenerateUniqueNumbers(int count)
        {
            var numbers = Enumerable.Range(1, count).ToList();
            for (var i = 0; i < numbers.Count; i++)
            {
                numbers[i] = i + 1;
            }
            return numbers;
        }

        private void SpawnDots(List<int> numbers)
        {
            var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            var instantiatedDots = new List<Dot>();

            for (var i = 0; i < _dotsCount; i++)
            {
                var dot = _gameManager.Instantiator.InstantiatePrefabForComponent<Dot>(dotPrefab, _gameArea);

                dot.SetSize(Random.Range(Constants.MinDotSize, Constants.MaxDotSize));
                dot.SetPosition(GetRandomPositionInRect(instantiatedDots, dot));
            
                var number = numbers[i];
                dot.SetText(number.ToString());
                dot.SetNumber(number);
                if (i + 1 == numbers.Count)
                {
                    dot.SetLast(true);   
                }
                _doTweenManager.PlayFadeAnimation(dot.gameObject, dot, DoTweenManager.FadeType.FadeIn);

                instantiatedDots.Add(dot);
            }
        }
    
        private Vector2 GetRandomPositionInRect(List<Dot> previouslyInstantiatedDots, Dot instantiatedDot)
        {
            var attempts = 0;
            var positionForInstantiation = new Vector2();

            _gameArea.GetWorldCorners(_gameAreaCorners);

            while (attempts <= MaxChecks)
            {
                var isTooClose = false;
                var randomX = Random.Range(_gameAreaCorners[0].x, _gameAreaCorners[3].x);
                var randomY = Random.Range(_gameAreaCorners[0].y, _gameAreaCorners[1].y);
            
                positionForInstantiation.x = randomX;
                positionForInstantiation.y = randomY;
            
                foreach (var currentDot in previouslyInstantiatedDots)
                {
                    if (Vector2.Distance(positionForInstantiation, currentDot.GetPosition()) < currentDot.GetSizeInWorldSpace())
                    {
                        isTooClose = true;
                        attempts++;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    instantiatedDot.SetSize(_defaultDotsSizeInPixels);
                    return positionForInstantiation;
                }

                if (attempts == MaxChecks)
                {
                    foreach (var previouslyInstantiatedDot in previouslyInstantiatedDots)
                    {
                        previouslyInstantiatedDot.SetSize(previouslyInstantiatedDot.GetDotSizeInPixelsX() - 1);
                    }
                    instantiatedDot.SetSize(instantiatedDot.GetDotSizeInPixelsX() - 1);
                    attempts = 0;
                }
            }
        
            return Vector2.zero;
        }
    }
}