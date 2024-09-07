using System.Collections.Generic;
using PathCreation;
using UnityEngine;


namespace Match3.Game.MapPet.Shared
{
    public class PetPath : MonoBehaviour
    {
        [SerializeField] private PathCreator path = default;
        [SerializeField] private PetPlaceHolder[] places = default;


        public VertexPath Path => path.path;
        
        
        public PetPlaceHolder GetRandomPlace(PetPlaceHolder specification)
        {
            List<PetPlaceHolder> foundedPlaces = new List<PetPlaceHolder>();
            foreach (var place in places)
            {
                if(place.placeSpecification == specification.placeSpecification)
                    foundedPlaces.Add(place);
            }

            return foundedPlaces.RandomOne();
        }
    }

}


