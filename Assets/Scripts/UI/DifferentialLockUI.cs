using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using MudLike.Vehicles.Components;

namespace MudLike.UI
{
    /// <summary>
    /// UI для отображения состояния блокировки дифференциалов
    /// </summary>
    public class DifferentialLockUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text frontDiffText;
        [SerializeField] private Text middleDiffText;
        [SerializeField] private Text rearDiffText;
        [SerializeField] private Text centerDiffText;
        
        [Header("Colors")]
        [SerializeField] private Color lockedColor = Color.red;
        [SerializeField] private Color unlockedColor = Color.green;
        
        private EntityManager _entityManager;
        private Entity _truckEntity;
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        private void Update()
        {
            UpdateDifferentialLockDisplay();
        }
        
        /// <summary>
        /// Обновляет отображение блокировки дифференциалов
        /// </summary>
        private void UpdateDifferentialLockDisplay()
        {
            // Находим грузовик игрока
            if (_truckEntity == Entity.Null)
            {
                var query = _entityManager.CreateEntityQuery(typeof(PlayerTag), typeof(TruckData));
                var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                
                if (entities.Length > 0)
                {
                    _truckEntity = entities[0];
                }
                
                entities.Dispose();
                return;
            }
            
            if (!_entityManager.Exists(_truckEntity))
            {
                _truckEntity = Entity.Null;
                return;
            }
            
            // Получаем данные грузовика
            var truckData = _entityManager.GetComponentData<TruckData>(_truckEntity);
            
            // Обновляем UI
            UpdateDiffText(frontDiffText, truckData.LockFrontDifferential, "Front Diff");
            UpdateDiffText(middleDiffText, truckData.LockMiddleDifferential, "Middle Diff");
            UpdateDiffText(rearDiffText, truckData.LockRearDifferential, "Rear Diff");
            UpdateDiffText(centerDiffText, truckData.LockCenterDifferential, "Center Diff");
        }
        
        /// <summary>
        /// Обновляет текст дифференциала
        /// </summary>
        private void UpdateDiffText(Text textElement, bool isLocked, string diffName)
        {
            if (textElement == null) return;
            
            textElement.text = $"{diffName}: {(isLocked ? "LOCKED" : "UNLOCKED")}";
            textElement.color = isLocked ? lockedColor : unlockedColor;
        }
    }
}