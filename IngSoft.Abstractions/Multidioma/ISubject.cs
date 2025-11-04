namespace IngSoft.Abstractions.Multidioma
{
    public interface ISubject
    {
        void Suscribir(IObserver observer);
        void Desuscribir(IObserver observer);
        void NotificarObservers();
    }
}
