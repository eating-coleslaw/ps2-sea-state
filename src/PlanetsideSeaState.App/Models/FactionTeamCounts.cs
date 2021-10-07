namespace PlanetsideSeaState.App.Models
{
    public class FactionTeamCounts
    {
        public int Total { get; private set; }
        public int Vs { get; private set; }
        public int Nc { get; private set; }
        public int Tr { get; private set; }
        public int Unknown { get; private set; }

        #region Addition
        public void AddVs(int addend)
        {
            Total += addend;
            Vs += addend;
        }

        public void AddNc(int addend)
        {
            Total += addend;
            Nc += addend;
        }

        public void AddTr(int addend)
        {
            Total += addend;
            Tr += addend;
        }

        public void AddUnknown(int addend)
        {
            Total += addend;
            Unknown += addend;
        }
        #endregion Addition

        #region Subtraction
        public void SubtractVs(int addend)
        {
            Total -= addend;
            Vs -= addend;
        }

        public void SubtractNc(int addend)
        {
            Total -= addend;
            Nc -= addend;
        }

        public void SubtractTr(int addend)
        {
            Total -= addend;
            Tr -= addend;
        }

        public void SubtractUnknown(int addend)
        {
            Total -= addend;
            Unknown -= addend;
        }
        #endregion Subtraction
    }
}
