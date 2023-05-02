#include <iostream>
#include <vector>
#include <algorithm>
#include <fstream>
#include <random>
#include <ctime>
#include <cmath>

int action_num = 8;
int state_num = 1024;
int epoch_num = 1000000;
double gamma = 0.9;
double alpha_max = 0.9;
double alpha_decay = 0.0005;
double epsilon_max = 1.0;
double epsilon_min = 0.1;
double epsilon_decay = 0.99995;

std::vector<std::vector<double>> Q, R;
std::default_random_engine random_gen = std::default_random_engine();
std::uniform_int_distribution<int> dist_step = std::uniform_int_distribution<int>(-1, 1);
std::uniform_real_distribution<double> dist_epsi = std::uniform_real_distribution<double>(0.0, 1.0);

void PrintArray(std::vector<std::vector<double>> arr, std::string filename) {
    std::ofstream file(filename);
    if (file.is_open()) {
        file << "Health,NeighNum,DistFood,DistSafe,DistEnemy,Flee,SeekSafe,SeekFood,Eat,Flock,Wander,Charge,Assist\n";
        for (int i = 0; i < arr.size(); i++) {
            file << ((i & 0b1100000000) >> 8) << ",";
            file << ((i & 0b0011000000) >> 6) << ",";
            file << ((i & 0b0000110000) >> 4) << ",";
            file << ((i & 0b0000001100) >> 2) << ",";
            file << (i & 0b0000000011) << ",";
            for (int j = 0; j < arr[i].size(); j++) {
                if (j == arr[i].size() - 1)
                    file << arr[i][j] << "\n";
                else
                    file << arr[i][j] << ",";
            }
        }
        file.close();
    }
}

double GetReward(int state, int action, int factor) {
    int de = (state & 0b0000000011);
    int ds = (state & 0b0000001100) >> 2;
    int df = (state & 0b0000110000) >> 4;
    int nn = (state & 0b0011000000) >> 6;
    int hp = (state & 0b1100000000) >> 8;
    switch (action) {
    case 0:
        return std::ceil((6 - hp + ds - de) / 9.0 * factor);
    case 1:
        return std::ceil((9 - hp - ds - de) / 9.0 * factor);
    case 2:
    case 3:
        return std::ceil((6 - hp - df) / 6.0 * factor);
    case 6:
    case 7:
        return std::ceil((3 + hp + nn - de) / 9.0 * factor);
    default:
        return 0;
    }
}

std::vector<std::vector<double>> RewardSetting(int state_num, int action_num) {
    std::vector<std::vector<double>> R(state_num, std::vector<double>(action_num, 0));
    for (int s = 0; s < state_num; s++) {
        int de = (s & 0b0000000011);
        int ds = (s & 0b0000001100) >> 2;
        int df = (s & 0b0000110000) >> 4;
        int nn = (s & 0b0011000000) >> 6;
        int hp = (s & 0b1100000000) >> 8;
        for (int a = 0; a < action_num; a++) {
            if (hp == 0)
                R[s][a] = -300;
            else if (a == 0) {//FLEE
                if (hp == 1 && ds > 1 && de < 2)
                    R[s][a] = GetReward(s, a, 20);
                else if (ds < 2 || de > 1)
                    R[s][a] = -1;
            }
            else if (a == 1) {//SEEKSAFE
                if (hp == 1 && ds == 1 && de < 2)
                    R[s][a] = GetReward(s, a, 300);
                else if (ds == 0 || de > 1)
                    R[s][a] = -1;
            }
            else if (a == 2) {//SEEKFOOD
                if (hp == 1 && df != 0)
                    R[s][a] = GetReward(s, a, 100);
                else if (hp == 3 || df == 0)
                    R[s][a] = -1;
            }
            else if (a == 3) {//EAT
                if (hp == 1 && df == 0)
                    R[s][a] = GetReward(s, a, 200);
                else if (hp == 3 || df != 0)
                    R[s][a] = -1;
            }
            else if (a == 4) {//FLOCK
                if (nn < 2 || de < 2)
                    R[s][a] = -1;
            }
            else if (a == 5) {//WANDER
                if (nn > 1 || de < 2)
                    R[s][a] = -1;
            }
            else if (a == 6) {//CHARGE
                if (nn == 3 && hp > 1 && de < 2)
                    R[s][a] = GetReward(s, a, 12);
                else
                    R[s][a] = -1;
            }
            else if (a == 7) {//ASSIST
                if (nn == 2 && hp > 1 && de < 2)
                    R[s][a] = GetReward(s, a, 10);
                else
                    R[s][a] = -1;
            }
        }
    }
    PrintArray(R, "./RTable.csv");
    return R;
}

std::pair<int, int> EpsilonGreedyPolicy(int state, double epsilon) {
    int action = -1;
    if (dist_epsi(random_gen) >= epsilon)
    {
        action = std::distance(Q[state].begin(), std::max_element(Q[state].begin(), Q[state].end()));
        if (R[state][action] < 0)
            return std::make_pair(-1, -1);
    }
    else
    {
        std::vector<int> indexs;
        for (int i = 0; i < R[state].size(); i++)
            if (R[state][i] >= 0)
                indexs.push_back(i);
        std::uniform_int_distribution<int> dist_len(0, indexs.size() - 1);
        action = indexs[dist_len(random_gen)];
        if (indexs.empty())
            return std::make_pair(-1, -1);
    }
    int de = (state & 0b0000000011);
    int ds = (state & 0b0000001100) >> 2;
    int df = (state & 0b0000110000) >> 4;
    int nn = (state & 0b0011000000) >> 6;
    int hp = (state & 0b1100000000) >> 8;
    int dep = 0, dsp = 0, dfp = 0, nnp = 0, hpp = 0, sp = 0;
    switch (action) {
    case 0:
        dep = de + 1;
        dsp = std::clamp(ds + dist_step(random_gen), 0, 3);
        dfp = std::clamp(df + dist_step(random_gen), 0, 3);
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = dep == 0 ? std::max(hp - 1, 0) : hp;
        break;
    case 1:
        dep = de;
        dsp = 0;
        dfp = std::clamp(df + dist_step(random_gen), 0, 3);
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = hp;
        break;
    case 2:
        dep = std::clamp(de + dist_step(random_gen), 0, 3);
        dsp = std::clamp(ds + dist_step(random_gen), 0, 3);
        dfp = std::max(df - 1, 0);
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = dep == 0 ? std::max(hp - 1, 0) : hp;
        break;
    case 3:
        dep = std::clamp(de + dist_step(random_gen), 0, 3);
        dsp = ds;
        dfp = df;
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = dep == 0 ? hp : std::min(hp + 1, 3);
        break;
    case 4:
    case 5:
        dep = std::clamp(de + dist_step(random_gen), 0, 3);
        dsp = std::clamp(ds + dist_step(random_gen), 0, 3);
        dfp = std::clamp(df + dist_step(random_gen), 0, 3);
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = dep == 0 ? std::max(hp - 1, 0) : hp;
        break;
    case 6:
    case 7:
        dep = std::max(de - 1, 0);
        dsp = std::clamp(ds + dist_step(random_gen), 0, 3);
        dfp = std::clamp(df + dist_step(random_gen), 0, 3);
        nnp = std::clamp(nn + dist_step(random_gen), 0, 3);
        hpp = dep == 0 ? std::max(hp - 1, 0) : hp;
        break;
    default:
        break;
    }
    sp = (hpp << 8) | (nnp << 6) | (dfp << 4) | (dsp << 2) | dep;
    return std::make_pair(sp, action);
}

int main(int argc, char* argv[]) {
    Q = std::vector<std::vector<double>>(state_num, std::vector<double>(action_num, 0));
    R = RewardSetting(state_num, action_num);
    std::clock_t start = clock(), end = clock();
    for (int epoch = 0; epoch < epoch_num; ++epoch) {
        end = clock();
        if (end - start >= 60000) {
            start = end;
            std::cout << epoch << "/" << epoch_num << std::endl;
        }
        double alpha = alpha_max / (1 + alpha_decay * epoch);
        double epsilon = std::max(epsilon_min, epsilon_max * std::pow(epsilon_decay, epoch));
        for (int state = 0; state < state_num; ++state) {
            for (int i = 0; i < 24; ++i) {
                std::pair<int, int> pair = EpsilonGreedyPolicy(state, epsilon);
                if (pair.first == -1)
                    break;
                double q_max = *std::max_element(Q[pair.first].begin(), Q[pair.first].end());
                Q[state][pair.second] *= 1 - alpha_max;
                Q[state][pair.second] += alpha_max * (R[state][pair.second] + gamma * q_max);
                state = pair.first;
            }
        }
    }
    PrintArray(Q, "./QTable.csv");
}