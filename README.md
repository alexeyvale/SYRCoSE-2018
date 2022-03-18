# LanD parser generator
This is the version of LanD parser generator referenced in [the paper](https://www.ispras.ru/proceedings/docs/2018/30/4/isp_30_2018_4_7.pdf) presented at SYRCoSE'18. 
## Prerequisites
[Java Runtime Environment](http://www.oracle.com/technetwork/java/javase/downloads/jre8-downloads-2133155.html) must be installed for correct lexical analyzer generation and work.

Missing NuGet packages will be automatically restored when the solution is built for the first time.
## Further research on tolerant parsing and binding to code:
Tolerant parsing using modified LR(1) and LL(1) algorithms with embedded “Any” symbol.
https://www.ispras.ru/proceedings/docs/2019/31/3/isp_31_2019_3_7.pdf
https://github.com/alexeyvale/SYRCoSE-2019
Using improved context-based code description for robust algorithmic binding to changing code
https://www.sciencedirect.com/science/article/pii/S1877050921020652
https://github.com/alexeyvale/YSC-2021
