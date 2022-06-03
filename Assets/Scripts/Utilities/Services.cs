using UnityEngine;
/*
 * Creator: Nate Smith
 * Creation Date: 2/6/2021
 * Description: A holder for systems that need to be referenced in many different scripts.
 * It is Static, so it does not need to be in the scene to work.
 * Needs to grab other systems.
 * Superior to a singleton instances because they are clumsy.
 * 
 * Using this form of Getters and Setters prevents crashes from null references.
 */

public static class Services
{
    #region Variables

    // Ensures you don't get a null reference exception.

    private static EventManager _eventManager;
    public static EventManager EventManager
    {
        get
        {
            Debug.Assert(_eventManager != null);
            return _eventManager;
        }
        private set => _eventManager = value;
    }

    #endregion


    #region Functions

    public static void InitializeServices()
    {
        EventManager = new EventManager();
    }
    
    #endregion
}