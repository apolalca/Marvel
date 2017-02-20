using Hero_Explorer.dal;
using Hero_Explorer.ent.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hero_Explorer.ui.ViewModel
{
        public class ViewModel : Base
        {
            private ObservableCollection<Character> _personajes;
            private ObservableCollection<ComicBook> _comics;
            private Character _personajeSeleccionado;
            private string _buscarPersonaje;
            private bool _isGettingData;
            private String _informacion;
            private ComicBook _comicSeleccionado;

            public ViewModel()
            {
                isGettingData = true;
                _personajes = obtenerListadoPersonajes();
                isGettingData = false;
            }

            public string Informacion
            {
                get
                {
                    return _informacion;
                }
                set
                {
                    _informacion = value;
                    NotifyPropertyChanged("Informacion");
                }
            }

            public bool isGettingData
            {
                get
                {
                    return _isGettingData;
                }
                set
                {
                    _isGettingData = value;
                    NotifyPropertyChanged("isGettingData");
                }
            }

            public String buscarPersonaje
            {
                get
                {
                    return _buscarPersonaje;
                }
                set
                {
                    isGettingData = true;
                    Personajes = new ObservableCollection<Character>();
                    Comics = new ObservableCollection<ComicBook>();
                    _buscarPersonaje = value;
                    NotifyPropertyChanged("buscarPersonaje");

                    if (_buscarPersonaje.Equals(""))
                        Personajes = obtenerListadoPersonajes();
                    else
                        Personajes = obtenerListadoPersonajes(_buscarPersonaje);

                    isGettingData = false;
                }
            }

            public ComicBook ComicSelccionado
            {
                get
                {
                    return _comicSeleccionado;
                }
                set
                {
                    _comicSeleccionado = value;
                    NotifyPropertyChanged("ComicSelccionado");
                }
            }

            public ObservableCollection<ComicBook> Comics
            {
                get
                {
                    return _comics;
                }
                set
                {
                    _comics = value;
                    NotifyPropertyChanged("Comics");
                }
            }

            public Character PersonajeSeleccionado
            {
                get
                {
                    return _personajeSeleccionado;
                }
                set
                {
                    _personajeSeleccionado = value;
                    //Si el personaje esta seleccionado y le damos a buscar el sistema pone este personaje a nulo y pasa por aqui por eso tenemos que poner el id
                    if (_personajeSeleccionado != null)
                        Comics = obtenerListadoComics(PersonajeSeleccionado.id);
                    NotifyPropertyChanged("PersonajeSeleccionado");
                }
            }

            public ObservableCollection<Character> Personajes
            {
                get
                {
                    return _personajes;
                }
                set
                {
                    _personajes = value;
                    NotifyPropertyChanged("Personajes");
                }
            }

            private ObservableCollection<ComicBook> obtenerListadoComics(int characterId)
            {
                ObservableCollection<ComicBook> comic = new ObservableCollection<ComicBook>();

                //IMPORTANTE: llamarlo desde un metodo async
                var res = Task.Run(() => Connection.getComicsData(characterId)).Result;

                if (res != null)
                    res.data.results.ForEach(x => comic.Add(x));

                return comic;
            }

            private ObservableCollection<Character> obtenerListadoPersonajes()
            {
                ObservableCollection<Character> pers = new ObservableCollection<Character>();

                //IMPORTANTE: llamarlo desde un metodo async
                var res = Task.Run(Connection.getCharacterData).Result;

                if (res != null)
                    res.data.results.ForEach(x => pers.Add(x));

                return pers;
            }

            private ObservableCollection<Character> obtenerListadoPersonajes(string starWith)
            {
                ObservableCollection<Character> pers = new ObservableCollection<Character>();

                //IMPORTANTE: llamarlo desde un metodo async
                var res = Task.Run(() => Connection.getCharacterData(starWith)).Result;

                if (res != null)
                    res.data.results.ForEach(x => pers.Add(x));

                if (res.data.results.Capacity == 0)
                    Informacion = "No se ha podidio encontrar el personaje";
                else
                    Informacion = "";

                return pers;
            }
        }
    }
