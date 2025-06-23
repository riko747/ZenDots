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
    
    public const int MaxChecks = 10;
    
    private void Start()
    {
        Debug.Log(
            $"Level: {_levelManager.GetCurrentLevel().levelNumber}, Dots: {_levelManager.GetCurrentLevel().dotCount}, Time: {_levelManager.GetCurrentLevel().timeLimit}");
        
        var dot = _resourcesManager.LoadEntity<Dot>(Constants.DOT_PREFAB_PATH);
        _defaultDotsSizeInPixels = dot.GetDotSizeInPixelsX;
        _gameAreaCorners = new Vector3[4];
        _dotsCount = _levelManager.GetCurrentLevel().dotCount;
        var instantiatedDots = new List<Dot>();
        var numbers = new HashSet<int>();

        while (numbers.Count < _levelManager.GetCurrentLevel().dotCount)
        {
            var randomNumber = Random.Range(1, _dotsCount + 1);
            numbers.Add(randomNumber);
        }
        
        for (var i = 0; i < _dotsCount; i++)
        {
            Dot instantiatedDot = _gameManager.Instantiator.InstantiatePrefabForComponent<Dot>(dot, gameArea);
            
            instantiatedDot.GetDotTransform.position = GetRandomPositionInRect(instantiatedDots, instantiatedDot);
            instantiatedDot.DotLabel.text = numbers.ToList().ElementAt(i).ToString();
            instantiatedDot.SetDotNumber(numbers.ToList().ElementAt(i));
            instantiatedDots.Add(instantiatedDot);
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
                _defaultDotsSizeInPixels -= 1;
                foreach (var previouslyInstantiatedDot in previouslyInstantiatedDots)
                {
                    previouslyInstantiatedDot.SetDotSize(_defaultDotsSizeInPixels);
                }
                instantiatedDot.SetDotSize(_defaultDotsSizeInPixels);
                attempts = 0;
            }
        }
        
        return Vector2.zero;
    }

    public int GetMaxDots() => _dotsCount;
}
