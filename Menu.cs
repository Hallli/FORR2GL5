using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
 public void Byrja(){
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
 }
 public void Restart(){
     SceneManager.LoadScene(0);
 }
}
