using System.Collections.Generic;
using Entities.Dot;
using Interfaces.Managers;
using Interfaces.Strategies;
using Managers;
using Other;
using UnityEngine;
using Zenject;

namespace Core.Spawners
{
    public class ZenModeDotSpawner : ISpawnStrategy
    {
        [Inject] private IResourcesManager _resourcesManager;
        [Inject] private IGameManager _gameManager;
        [Inject] private IDoTweenManager _doTweenManager;
        
        private Vector3[] _gameAreaCorners;
        private float _defaultDotsSizeInPixels;
        private readonly RectTransform _gameArea;
        private int _nextDotNumber;

        private const int MaxChecks = 100;
        private const int DotsStartCount = 9;

        private readonly List<Dot> _instantiatedDots = new();
        

        public ZenModeDotSpawner(RectTransform gameArea)
        {
            _gameArea = gameArea;
        }
        
        public void Spawn()
        {
            InitializeFields();
            _gameManager.OnRightDotClicked += AddNewDot;
            SpawnDots(DotsStartCount);
        }

        private void InitializeFields()
        {
            var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            _defaultDotsSizeInPixels = dotPrefab.GetDotSizeInPixelsX();
            _gameAreaCorners = new Vector3[4];
            _nextDotNumber = DotsStartCount + 1;
        }

        private void SpawnDots(int number, bool newDot = false, int newDotNumber = 0)
        {
            var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DotPrefabPath);
            for (var i = 0; i != number; i++)
            {
                var dot = _gameManager.Instantiator.InstantiatePrefabForComponent<Dot>(dotPrefab, _gameArea);

                dot.SetSize(Random.Range(Constants.MinDotSize, Constants.MaxDotSize));
                dot.SetPosition(GetRandomPositionInRect(_instantiatedDots, dot));

                if (newDot)
                {
                    dot.SetText(newDotNumber.ToString());
                    dot.SetNumber(newDotNumber);
                    _nextDotNumber++;
                }
                else
                {
                    dot.SetText((i + 1).ToString());
                    dot.SetNumber(i + 1);
                    _instantiatedDots.Add(dot);
                }

                _doTweenManager.PlayFadeAnimation(dot.gameObject, dot, DoTweenManager.FadeType.FadeIn);
            }
        }

        private void AddNewDot()
        {
            foreach (var instantiatedDot in _instantiatedDots)
            {
                if (instantiatedDot.IsDeactivated)
                {
                    instantiatedDot.SetSize(Random.Range(Constants.MinDotSize, Constants.MaxDotSize));
                    instantiatedDot.SetPosition(GetRandomPositionInRect(_instantiatedDots, instantiatedDot));

                    instantiatedDot.SetText(_nextDotNumber.ToString());
                    instantiatedDot.SetNumber(_nextDotNumber);
                    _doTweenManager.PlayPopInAnimation(instantiatedDot.GetTransform(), instantiatedDot);
                    _nextDotNumber++;
                    return;
                }
            }
            SpawnDots(1, true, _nextDotNumber);
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
            
                foreach (var previouslyInstantiatedDot in previouslyInstantiatedDots)
                {
                    if (previouslyInstantiatedDot.IsDeactivated || previouslyInstantiatedDot.IsPending) continue;

                    if (!(Vector2.Distance(positionForInstantiation, previouslyInstantiatedDot.GetPosition()) <
                          previouslyInstantiatedDot.GetSizeInWorldSpace())) continue;
                    
                    isTooClose = true;
                    attempts++;
                    break;
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