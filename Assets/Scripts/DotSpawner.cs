using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Other;
using UnityEngine;
using Zenject;

public class DotSpawner : MonoBehaviour
{
    [Inject] private IResourcesManager _resourcesManager;
    [Inject] private IGameManager _gameManager;
    
    [SerializeField] private RectTransform gameArea;

    private Vector3[] _gameAreaCorners;
    private float _defaultDotsSizeInPixels;
    
    public const int MaxDots = 10;
    public const int MaxChecks = 10;
    
    private void Start()
    {
        var dot = _resourcesManager.LoadEntity<Dot>(Constants.DOT_PREFAB_PATH);
        _defaultDotsSizeInPixels = dot.GetDotSizeInPixelsX;
        _gameAreaCorners = new Vector3[4];
        var instantiatedDots = new List<Dot>();
        var numbers = new HashSet<int>();

        while (numbers.Count < MaxDots)
        {
            var randomNumber = Random.Range(1, MaxDots + 1);
            numbers.Add(randomNumber);
        }
        
        for (var i = 0; i < MaxDots; i++)
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

    public int GetMaxDots() => MaxDots;
}
