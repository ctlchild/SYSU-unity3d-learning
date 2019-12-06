#include<bits/stdc++.h>
#define rep(i,l,r) for (int i=l;i<=r;i++)
using namespace std;
const int inf=1e9;
int a[200][200],dis[200],vis[200];
int get(int o,int i,int j){
    return o*16+i*4+j;
}
bool jud(int o,int i,int j){
    if (i<0||i>3||j<0||j>3) return 0;
    if ((i<j)&&(i!=0)) return 0;
    if ((3-i<3-j)&&(3-i!=0)) return 0;
    return 1;
}
int main(){
    rep(o,0,1){
        rep(i,0,3){
            rep(j,0,3){
                int st=get(o,i,j);
                if (!jud(o,i,j)) continue;
                if (o==0){  //from
                    rep(dx,-2,0) rep(dy,-2,0) if (1<=abs(dx+dy)&&abs(dx+dy)<=2){
                        if (!jud(o^1,i+dx,j+dy)) continue;
                        a[get(o,i,j)][get(o^1,i+dx,j+dy)]=1;
                    }
                }
                else {
                    rep(dx,0,2) rep(dy,0,2) if (1<=dx+dy&&dx+dy<=2){
                        if (!jud(o^1,i+dx,j+dy)) continue;
                        a[get(o,i,j)][get(o^1,i+dx,j+dy)]=1;
                    }
                }
            }
        }
    }
    // rep(i,0,3) rep(j,0,3){
    //     rep(x,0,3) rep(y,0,3) if (a[get(i,j)][get(x,y)]==1) printf("%d %d %d %d\n",i,j,x,y);
    // }
    rep(o,0,1) rep(i,0,3) rep(j,0,3) dis[get(o,i,j)]=inf; 
    dis[get(1,0,0)]=0;
    queue<int> q; q.push(get(1,0,0)); vis[get(1,0,0)]=1; 
    while (!q.empty()){
        int u=q.front(); q.pop(); vis[u]=0;
        int uo=u/16,ux=(u%16)/4,uy=u%4;
        rep(i,-2,2) rep(j,-2,2) {
            int vo=uo^1,vx=ux+i,vy=uy+j;
            int v=get(vo,ux+i,uy+j);
            if (jud(vo,vx,vy)&&a[v][u]&&dis[v]>dis[u]+1){
                dis[v]=dis[u]+1;
                if (!vis[v]) vis[v]=1,q.push(v);
            }
        }
    }
    rep(o,0,1) rep(i,0,3) rep(j,0,3){
        rep(x,0,3) rep(y,0,3) {
            if (dis[get(o,x,y)]==dis[get(o^1,i,j)]+1&&a[get(o,x,y)][get(o^1,i,j)]) a[get(o,x,y)][get(o^1,i,j)]=1;
            else a[get(o,x,y)][get(o^1,i,j)]=0;
        }
    }
    rep(o,0,1) rep(i,0,3) rep(j,0,3){
        printf("{");
        rep(no,0,1) rep(x,0,3) rep(y,0,3) {
            printf("%d",a[get(o,i,j)][get(no,x,y)]);
            if (x==3&&y==3&&no==1) printf("},\n");
            else printf(",");
        }
    }
    return 0;
}