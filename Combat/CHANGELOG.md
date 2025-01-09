~~~~# COMBAT Changelog

## 1.0.0 - Initial 1.6 release

### Changed

* **Changes to speed formula:**
    * ```S = exp(-k * tanh((a * w + b * p) / m)``` where `w` is the weapon's innate speed stat and `p` is the player's attack speed stat. `k = 2`, `a = 0.2`, `b = 10` and `m = 20` are constant.
* **Changes to damage mitigation formula:**
    * ```D *= 0.9 ^ d```, where `d` is the sum of the weapon's innate defense and the player's defense stat.

[🔼 Back to top](#combat-changelog)
