using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using Other;
using UnityEngine;
using Zenject;

public class DotSpawner : MonoBehaviour
{
    [Inject] private IResourcesManager _resourcesManager;
    [Inject] private IGameManager _gameManager;
    [Inject] private LevelManager _levelManager;
    
    [SerializeField] private RectTransform gameArea;

    private Vector3[] _gameAreaCorners;
    private float _defaultDotsSizeInPixels;
    private int _dotsCount;

    private const int MaxChecks = 100;
    
    private void Start()
    {
        LogLevelInfo();
        InitializeFields();

        var numbers = GenerateUniqueNumbers(_dotsCount);
        SpawnDots(numbers);
    }

    private void LogLevelInfo()
    {
        var level = _levelManager.GetCurrentLevel();
        Debug.Log($"Level: {level.levelNumber}, Dots: {level.dotCount}, Time: {level.timeLimit}");
    }

    private void InitializeFields()
    {
        var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DOT_PREFAB_PATH);
        _defaultDotsSizeInPixels = dotPrefab.GetDotSizeInPixelsX;
        _gameAreaCorners = new Vector3[4];
        _dotsCount = _levelManager.GetCurrentLevel().dotCount;
    }

    private List<int> GenerateUniqueNumbers(int count)
    {
        var numbers = Enumerable.Range(1, count).ToList();
        for (var i = 0; i < numbers.Count; i++)
        {
            var randIndex = Random.Range(i, numbers.Count);
            (numbers[i], numbers[randIndex]) = (numbers[randIndex], numbers[i]);
        }
        return numbers;
    }

    private void SpawnDots(List<int> numbers)
    {
        var dotPrefab = _resourcesManager.LoadEntity<Dot>(Constants.DOT_PREFAB_PATH);
        var instantiatedDots = new List<Dot>();

        for (var i = 0; i < _dotsCount; i++)
        {
            var dot = _gameManager.Instantiator.InstantiatePrefabForComponent<Dot>(dotPrefab, gameArea);

            dot.SetDotSize(Random.Range(dot.GetMinDotSize(), dot.GetMaxDotSize()));
            dot.GetDotTransform.position = GetRandomPositionInRect(instantiatedDots, dot);

            var number = numbers[i];
            dot.DotLabel.text = number.ToString();
            dot.SetDotNumber(number);

            instantiatedDots.Add(dot);
        }
    }
    
    private Vector2 GetRandomPositionInRect(List<Dot> previouslyInstantiatedDots, Dot instantiatedDot)
    {
        var attempts = 0;
        var positionForInstantiation = new Vector2();

        gameArea.GetWorldCorners(_gameAreaCorners);

        while (attempts <= MaxChecks)
        {
            var isTooClose = false;
            var randomX = Random.Range(_gameAreaCorners[0].x, _gameAreaCorners[3].x);
            var randomY = Random.Range(_gameAreaCorners[0].y, _gameAreaCorners[1].y);
            
            positionForInstantiation.x = randomX;
            positionForInstantiation.y = randomY;
            
            foreach (var currentDot in previouslyInstantiatedDots)
            {
                if (Vector2.Distance(positionForInstantiation, currentDot.GetDotPosition) < currentDot.GetDotSizeInWorldSpace)
                {
                    isTooClose = true;
                    attempts++;
                    break;
                }
            }

            if (!isTooClose)
            {
                instantiatedDot.SetDotSize(_defaultDotsSizeInPixels);
                return positionForInstantiation;
            }

            if (attempts == MaxChecks)
            {
                foreach (var previouslyInstantiatedDot in previouslyInstantiatedDots)
                {
                    previouslyInstantiatedDot.SetDotSize(previouslyInstantiatedDot.GetDotSizeInPixelsX - 1);
                }
                instantiatedDot.SetDotSize(instantiatedDot.GetDotSizeInPixelsX - 1);
                attempts = 0;
            }
        }
        
        return Vector2.zero;
    }

    public int GetMaxDots() => _dotsCount;
}
