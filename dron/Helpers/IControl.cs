namespace dron
{
    interface IControl
    {
        int Roll { get; }
        int Pitch { get; }
        int Gaz { get; }
        int Yaw { get; }
        void Refresh();
    }
}
