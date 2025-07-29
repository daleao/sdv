# COMBAT Changelog

## 1.1.0

### Changed

* Player defense (i.e., from rings) and weapon defense now stack multiplicatively instead of additively. This means that 1 point of defense from a ring and 1 point from a weapon will mitigate more damage than 2 points from just rings.

### Fixed

* Fixed hyperbolic mitigation formula not applying correctly.
* Fixed Defense Book not applying with the hyperbolic formula.

## 1.0.1

### Added

* Added Chinese translations by [BlackRosePetals](https://github.com/BlackRosePetals).

## 1.0.0 - Initial 1.6 release

### Changed

* **Changes to speed formula:**
    * ```S = exp(-k * tanh((a * w + b * p) / m)``` where `w` is the weapon's innate speed stat and `p` is the player's attack speed stat. `k = 2`, `a = 0.2`, `b = 10` and `m = 20` are constant.

[🔼 Back to top](#combat-changelog)
