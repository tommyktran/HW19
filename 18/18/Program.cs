using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace _18
{
    class Program
    {
        static void Main(string[] args)
        {
            var ballot = new Ballot("0002");
            ballot.Output();
            Vote(ballot);
        }

        static void Vote(Ballot ballot)
        {
            Contest currentContest = ballot.contests[ballot.currentContestIndex];
            Candidate currentCandidate = ballot.contests[ballot.currentContestIndex].candidates[ballot.currentCandidateIndex];
            bool isDone = false;

            Console.Write(currentContest.name + " --- " + currentCandidate.fullName);
            if (currentCandidate.party == "WRITE-IN")
            {
                Console.Write(" (WRITE-IN)");
            }
            if (currentCandidate.isSelected)
            {
                Console.WriteLine(" (Selected)");
            }
            else
            {
                Console.WriteLine();
            }

            Console.Write("Press a key --  ");
            List<int> options = new List<int>();

            options.Add(0);
            Console.Write("0: Display Ballot  ");

            if (ballot.currentContestIndex != 0)
            {
                options.Add(2);
                Console.Write("2: Prev Contest  ");
            }
            if (ballot.currentCandidateIndex != 0)
            {
                options.Add(4);
                Console.Write("4: Prev Candidate  ");
            }
            if (currentCandidate.isSelected)
            {
                options.Add(5);
                Console.Write("5: Deselect  ");
            }
            else
            {
                options.Add(5);
                Console.Write("5: Select  ");
            }

            if (ballot.currentCandidateIndex != currentContest.candidates.Count - 1)
            {
                options.Add(6);
                Console.Write("6: Next Candidate  ");
            }

            if (ballot.currentContestIndex != ballot.contests.Count - 1)
            {
                options.Add(8);
                Console.Write("8: Next Contest  ");
            }
            else
            {
                isDone = true;
                options.Add(8);
                Console.Write("8: Done  ");
            }
            Console.WriteLine();
            int input = ReadKey();
            Console.WriteLine();


            if (!options.Contains(input))
            {
                Console.WriteLine("Invalid input!");
                Console.WriteLine();
                Vote(ballot);
                return;
            }

            if (input == 0)
            {
                ballot.Output();
            }
            if (input == 2)
            {
                ballot.currentContestIndex--;
                ballot.currentCandidateIndex = 0;
            }
            if (input == 4)
            {
                ballot.currentCandidateIndex--;
            }
            if (input == 5)
            {
                if (currentCandidate.isSelected)
                {
                    if (currentCandidate.party == "WRITE-IN")
                    {
                        currentCandidate.fullName = "";
                    }
                    currentCandidate.isSelected = false;
                }
                else
                {
                    if (currentContest.GetHowManySelected() >= currentContest.numberToVote)
                    {
                        Console.WriteLine("Overvote!");
                    }
                    else
                    {
                        if (currentCandidate.party == "WRITE-IN")
                        {
                            Console.WriteLine("Enter the write-in name: ");
                            string nameInput = Console.ReadLine().Trim();
                            if (nameInput == "")
                            {
                                Console.WriteLine("Invalid name");
                            }
                            else
                            {
                                currentCandidate.fullName = nameInput;
                                currentCandidate.isSelected = true;
                            }
                        }
                    }
                    currentCandidate.isSelected = true;
                }
            }
            if (input == 6)
            {
                ballot.currentCandidateIndex++;
            }
            if (input == 8)
            {
                if (isDone)
                {
                    return;
                }
                else
                {
                    ballot.currentContestIndex++;
                    ballot.currentCandidateIndex = 0;
                }
            }

            Console.WriteLine();
            Vote(ballot);
        }

        static int ReadKey()
        {
            while (true)
            {
                char ch = Console.ReadKey().KeyChar;
                int result;

                if (int.TryParse(ch.ToString(), out result))
                {
                    return result;
                }
            }
        }
        public class Ballot
        {
            public string name;
            public List<Contest> contests = new List<Contest>();
            public List<string> candidateCodes = new List<string>();
            public int currentContestIndex;
            public int currentCandidateIndex;

            //public Ballot(string newname)
            //{
            //    name = newname;
            //    currentContestIndex = 0;
            //    currentCandidateIndex = 0;
            //}
            public Ballot(string newBallotCode)
            {
                string data = File.ReadAllText("BALLOT_" + newBallotCode + ".json");
                JsonDocument doc = JsonDocument.Parse(data);
                name = newBallotCode;
                var thing = doc.RootElement.GetProperty("Contests");
                for (int i = 0; i < thing.GetArrayLength(); i++)
                {
                    contests.Add(new Contest(thing[i].GetProperty("ContestCode").GetString(), 
                        thing[i].GetProperty("CandidateCodes")));
                }
            }

            public void AddContest(Contest contest)
            {
                contests.Add(contest);
            }

            public void Output()
            {
                foreach (Contest contest in contests)
                {
                    Console.WriteLine("Contest " + (contests.IndexOf(contest) + 1) + " of " + contests.Count + ": " + contest.name);
                    foreach (Candidate candidate in contest.candidates)
                    {
                        Console.Write("     ");
                        if (candidate.party == "WRITE-IN")
                        {
                            Console.Write("Write-in: " + candidate.fullName);
                        }
                        else
                        {
                            Console.Write(candidate.fullName + " (" + candidate.party + ")");
                        }

                        if (candidate.isSelected)
                        {
                            Console.WriteLine(" (Selected)");
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        public class Contest
        {
            public string name;
            public string contestCode;
            public int numberToVote;
            public List<Candidate> candidates = new List<Candidate>();
            public Contest(string newContestCode, string newname, int newNumber)
            {
                contestCode = newContestCode;
                name = newname;
                numberToVote = newNumber;
            }
            public Contest(string newContestCode, JsonElement candidateCodes)
            {
                string data = File.ReadAllText("CONTEST_" + newContestCode + ".json");
                JsonDocument doc = JsonDocument.Parse(data);

                contestCode = newContestCode;
                name = doc.RootElement.GetProperty("ContestName").GetString();
                numberToVote = doc.RootElement.GetProperty("MaxChoices").GetInt32();

                var candidatesArray = doc.RootElement.GetProperty("Candidates");
                for (int y = 0; y < doc.RootElement.GetProperty("Candidates").GetArrayLength(); y++)
                {
                    for (int i = 0; i < doc.RootElement.GetProperty("Candidates").GetArrayLength(); i++)
                    {
                        if (candidatesArray[i].GetProperty("CandidateCode").GetString() == candidateCodes[y].GetString())
                        {
                            string newCandidateCode = candidatesArray[i].GetProperty("CandidateCode").GetString();
                            string newCandidateName = candidatesArray[i].GetProperty("CandidateName").GetString();
                            string newCandidateParty = candidatesArray[i].GetProperty("CandidateParty").GetString();

                            candidates.Add(new Candidate(newCandidateName, newCandidateName, newCandidateParty, newCandidateCode));
                            i = doc.RootElement.GetProperty("Candidates").GetArrayLength();
                        }
                    }
                }

            }

            public void AddCandidate(Candidate candidate)
            {
                candidates.Add(candidate);
            }

            public void AddBlankWriteIns()
            {
                for (int x = numberToVote; x > 0; x--)
                {
                    AddCandidate(new Candidate("", "", "WRITE-IN"));
                }
            }

            public int GetHowManySelected()
            {
                int result = 0;
                foreach (var candidate in candidates)
                {
                    if (candidate.isSelected)
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        public class Candidate
        {
            public string shortName;
            public string fullName;
            public string party;
            public string candidateCode;
            public bool isSelected;
            public Candidate(string newSName, string newFName, string newParty)
            {
                shortName = newSName;
                fullName = newFName;
                party = newParty;
            }
            public Candidate(string newSName, string newFName, string newParty, string newCandidateCode)
            {
                shortName = newSName;
                fullName = newFName;
                party = newParty;
                candidateCode = newCandidateCode;
            }
        }

    }
}
