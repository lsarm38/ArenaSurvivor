using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBarUI : MonoBehaviour
{
	[SerializeField] private PlayerHealth playerHealth;

	private Slider slider;

	private void Awake()
	{
		slider = GetComponent<Slider>();
	}

	private void OnEnable()
	{
		if (playerHealth != null)
			playerHealth.OnHealthChanged += HandleHealthChanged;
	}

	private void OnDisable()
	{
		if (playerHealth != null)
			playerHealth.OnHealthChanged -= HandleHealthChanged;
	}

	private void HandleHealthChanged(float current, float max)
	{
		slider.maxValue = max;
		slider.value = current;
	}
}a