using System;
using System.Collections.Generic;
using UnityEngine;

// 1. Les noms d'événements sécurisés
public enum GameEvent
{
    OnStartAction,
    OnPlayerHealthChange,
    OnLevelUp,
    OnRegenerateStart,
    OnRegenerateEnd,
}

public class EventEmitter : MonoBehaviour
{
    // Dictionnaire pour stocker les abonnements. 
    // Key: Le nom de l'événement (Enum).
    // Value: La méthode à appeler (Action, ici sans argument pour la simplicité).
    private readonly Dictionary<GameEvent, Action> eventDictionary = new();

    // Rendre l'EventEmitter un Singleton si vous voulez un accès global
    public static EventEmitter Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // --- Méthode .on() (Abonnement) ---
    public void On(GameEvent eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            // Ajouter le nouveau listener (+=) à la liste existante
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            // Créer une nouvelle liste de listeners pour cet événement
            eventDictionary.Add(eventName, listener);
        }
        Debug.Log($"Abonné à l'événement : {eventName}");
    }

    // --- Méthode .emit() (Déclenchement) ---
    public void Emit(GameEvent eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            // Déclencher toutes les méthodes abonnées
            // L'Action est nulle si personne n'est abonné, mais TryGetValue gère déjà ça
            Debug.Log($"EMIT : {eventName}");
            thisEvent?.Invoke();
        }
    }

    // --- Méthode .off() (Désabonnement) ---
    public void Off(GameEvent eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }
}