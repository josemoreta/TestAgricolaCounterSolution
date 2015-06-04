using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAgricolaCounterApp
{
    public class AgricolaCalculatorViewModel : ViewModel
    {
        private int fields;
        private int pastures;
        private int fieldsScore;
        private int pasturesScore;
        private Dictionary<int, int> fieldsCountScore;
        private Dictionary<int, int> pasturesCountScore;
        private int totalScore;

        public int Fields
        {
            get
            {
                return this.fields;
            }
            set
            {
                if (value > 0)
                {
                    this.fields = value;
                    this.UpdateFieldsScore();
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int Pastures
        {
            get
            {
                return this.pastures;
            }
            set
            {
                this.pastures = value;
                this.UpdatePasturesScore();
                this.NotifyPropertyChanged();
            }
        }

        public int FieldsScore
        {
            get
            {
                return this.fieldsScore;
            }
            set
            {
                this.fieldsScore = value;
                this.NotifyPropertyChanged();
            }
        }

        public int PasturesScore
        {
            get
            {
                return this.pasturesScore;
            }
            set
            {
                this.pasturesScore = value;
                this.NotifyPropertyChanged();
            }
        }

        public int TotalScore
        {
            get
            {
                return this.totalScore;
            }
            set
            {
                this.totalScore = value;
                this.NotifyPropertyChanged();
            }
        }

        public RelayCommand AddField { get; set; }
        public RelayCommand RemoveField { get; set; }

        public RelayCommand AddPasture { get; set; }
        public RelayCommand RemovePasture { get; set; }

        public AgricolaCalculatorViewModel()
        {
            this.fieldsCountScore = new Dictionary<int, int>
            {
                {0, -1},
                {1, -1},
                {2, 1},
                {3, 2},
                {4, 3},
                {5, 4},
            };

            this.pasturesCountScore = new Dictionary<int, int>
            {
                {0, -1},
                {1, 1},
                {2, 2},
                {3, 3},
                {4, 4},
            };

            this.UpdateFieldsScore();
            this.UpdatePasturesScore();

            this.AddField = new RelayCommand(() => { this.Fields++; });
            this.RemoveField = new RelayCommand(() => { this.Fields--; });

            this.AddPasture = new RelayCommand(() => { this.Pastures++; });
            this.RemovePasture = new RelayCommand(() => { this.Pastures--; });
        }

        private void UpdateFieldsScore()
        {
            this.UpdateItemScore(this.Fields, this.fieldsCountScore, (i) => { this.FieldsScore = i; });
        }

        private void UpdatePasturesScore()
        {
            this.UpdateItemScore(this.Pastures, this.pasturesCountScore, (i) => { this.PasturesScore = i; });
        }

        private void UpdateItemScore(int item, Dictionary<int, int> itemCountScore, Action<int> setScore)
        {
            var score = -1;
            if (!itemCountScore.TryGetValue(item, out score))
            {
                score = itemCountScore.Values.Max();
            }

            setScore(score);

            this.UpdateTotalScore();
        }

        private void UpdateTotalScore()
        {
            this.TotalScore = this.FieldsScore + this.PasturesScore;
        }

    }
}
